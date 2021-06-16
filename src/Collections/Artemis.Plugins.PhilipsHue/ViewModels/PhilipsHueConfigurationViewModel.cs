using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Artemis.Core;
using Artemis.Plugins.PhilipsHue.Models;
using Artemis.Plugins.PhilipsHue.Services;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using Stylet;

namespace Artemis.Plugins.PhilipsHue.ViewModels
{
    public class PhilipsHueConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IHueService _hueService;
        private readonly IDialogService _dialogService;
        private readonly PluginSetting<int> _pollingRateSetting;
        private readonly PluginSetting<List<PhilipsHueBridge>> _storedBridgesSetting;
        private bool _foundNewBridge;
        private string _lightDisplay;
        private bool _locatingBridges;
        private PhilipsHueBridge _newBridge;
        private int _pollingRate;
        private string _roomDisplay;
        private int _wizardPage;
        private bool _showManualFind;


        public PhilipsHueConfigurationViewModel(Plugin plugin,
            PluginSettings settings,
            IHueService hueService,
            IDialogService dialogService,
            IModelValidator<PhilipsHueConfigurationViewModel> validator)
            : base(plugin, validator)
        {
            _hueService = hueService;
            _dialogService = dialogService;

            _storedBridgesSetting = settings.GetSetting("Bridges", new List<PhilipsHueBridge>());
            _pollingRateSetting = settings.GetSetting("PollingRate", 2000);
            // Reset to default if the setting is below 100ms because the scale changed from seconds to milliseconds
            if (_pollingRateSetting.Value < 100)
                _pollingRateSetting.Value = 2000;

            PollingRate = _pollingRateSetting.Value;

            if (_storedBridgesSetting.Value.Any())
                WizardPage = 3;
        }


        public int PollingRate
        {
            get => _pollingRate;
            set => SetAndNotify(ref _pollingRate, value);
        }

        public bool LocatingBridges
        {
            get => _locatingBridges;
            set => SetAndNotify(ref _locatingBridges, value);
        }

        public bool FoundNewBridge
        {
            get => _foundNewBridge;
            set => SetAndNotify(ref _foundNewBridge, value);
        }

        public int WizardPage
        {
            get => _wizardPage;
            set => SetAndNotify(ref _wizardPage, value);
        }

        public string RoomDisplay
        {
            get => _roomDisplay;
            set => SetAndNotify(ref _roomDisplay, value);
        }

        public string LightDisplay
        {
            get => _lightDisplay;
            set => SetAndNotify(ref _lightDisplay, value);
        }

        public void ReloadDeviceProvider()
        {
            HueDeviceProvider feature = Plugin.GetFeature<HueDeviceProvider>();
            if (feature == null || !feature.IsEnabled)
                return;

            // Take this off the UI thread
            Task.Run(() =>
            {
                feature.Disable();
                Thread.Sleep(100);
                feature.Enable();
            });
        }

        #region Settings

        public void AddHueBridge()
        {
            WizardPage = 0;
        }

        public async void ResetPlugin()
        {
            bool result = await _dialogService.ShowConfirmDialogAt(
                "PluginSettingsDialog",
                "Reset Philips Hue Plugin",
                "Are you sure about this? You'll have to reconnect with your Hue Bridge to continue using the plugin."
            );
            if (!result)
                return;

            _storedBridgesSetting.Value.Clear();
            _storedBridgesSetting.Save();

            WizardPage = 0;
            ReloadDeviceProvider();
        }

        #endregion

        #region Bridge discovery

        public async Task FindHueBridge()
        {
            if (LocatingBridges)
                return;

            LocatingBridges = true;
            IBridgeLocator locator = new HttpBridgeLocator();
            List<LocatedBridge> bridges = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).ToList();

            // Lets try to find some more 
            locator = new SsdpBridgeLocator();
            IEnumerable<LocatedBridge> extraBridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            foreach (LocatedBridge extraBridge in extraBridges)
            {
                if (bridges.All(b => b.IpAddress != extraBridge.IpAddress))
                    bridges.Add(extraBridge);
            }

            await Task.Delay(500);

            LocatedBridge newBridge = bridges.FirstOrDefault(b => _storedBridgesSetting.Value.All(s => s.IpAddress != b.IpAddress));
            if (newBridge == null)
            {
                FoundNewBridge = false;
                LocatingBridges = false;
                _newBridge = null;
                return;
            }

            FoundNewBridge = true;
            LocatingBridges = false;
            _newBridge = new PhilipsHueBridge(newBridge);

            if (FoundNewBridge)
                await RegisterHueBridge();
        }

        public async Task FindHueBridgeManual()
        {
            if (LocatingBridges)
                return;

            object result = await _dialogService.ShowDialogAt<PhilipsHueBridgeDialogViewModel>("PluginSettingsDialog");
            if (result is not PhilipsHueBridge newBridge)
                return;

            if (_storedBridgesSetting.Value.Any(b => b.IpAddress == newBridge.IpAddress))
            {
                await ShowAddBridgeError($"A Hue Bridge with IP address {newBridge.IpAddress} is already added");
                return;
            }

            LocatingBridges = true;

            using HttpClient httpClient = new();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"http://{newBridge.IpAddress}/description.xml");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await ShowAddBridgeError($"Failed to connect to Hue Bridge at {newBridge.IpAddress}");
                LocatingBridges = false;
                return;
            }

            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!response.Contains("philips hue bridge", StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowAddBridgeError($"Device at {newBridge.IpAddress} does not appear to be a Hue Bridge");
                LocatingBridges = false;
                return;
            }

            FoundNewBridge = true;
            LocatingBridges = false;
            _newBridge = newBridge;

            await RegisterHueBridge();
        }

        private async Task RegisterHueBridge()
        {
            WizardPage = 1;

            ILocalHueClient client = new LocalHueClient(_newBridge.IpAddress);
            RegisterEntertainmentResult registrationResult = null;
            int attempts = 0;

            void OnClosed(object sender, CloseEventArgs args)
            {
                attempts = 12;
            }

            Closed += OnClosed;
            while (registrationResult == null && attempts < 12)
            {
                try
                {
                    registrationResult = await client.RegisterAsync("artemis-hue-plugin", Environment.MachineName, true);
                }
                catch (LinkButtonNotPressedException)
                {
                    // ignored, user has not yet pressed sync button
                }

                attempts++;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Closed -= OnClosed;
            if (registrationResult == null)
            {
                // TODO handle
                WizardPage = 0;
                return;
            }

            string bridgeId = (await client.GetBridgeAsync())?.Config?.BridgeId;
            if (bridgeId == null)
            {
                await ShowAddBridgeError("Failed to retrieve Bridge ID");
                WizardPage = 0;
                return;
            }

            _newBridge.AppKey = registrationResult.Username;
            _newBridge.StreamingClientKey = registrationResult.StreamingClientKey;
            _newBridge.BridgeId = bridgeId.ToLowerInvariant();
            _storedBridgesSetting.Value.Add(_newBridge);
            _storedBridgesSetting.Save();

            FinishWizard();
            ReloadDeviceProvider();
        }

        private async void FinishWizard()
        {
            WizardPage = 2;

            ILocalHueClient client = new LocalHueClient(_newBridge.IpAddress);
            client.Initialize(_newBridge.AppKey);

            IReadOnlyCollection<Group> groups = await client.GetGroupsAsync();
            IEnumerable<Light> lights = await client.GetLightsAsync();

            int roomCount = groups.Count(g => g.Type == GroupType.Room);
            int lightCount = lights.Count();

            RoomDisplay = roomCount + (roomCount == 1 ? " room" : " rooms");
            LightDisplay = lightCount + (lightCount == 1 ? " light" : " lights");

            _newBridge = null;
        }

        private async Task ShowAddBridgeError(string error)
        {
            await _dialogService.ShowConfirmDialogAt("PluginSettingsDialog", "Add Hue bridge", error, "Confirm", null);
        }

        #endregion

        #region Lifecycle

        protected override void OnInitialActivate()
        {
            PropertyChanged += OnPropertyChanged;
            base.OnInitialActivate();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(PollingRate))
                return;

            bool valid = Validate();
            if (valid)
            {
                _pollingRateSetting.Value = PollingRate;
                _pollingRateSetting.Save();
            }
        }

        #endregion
    }
}