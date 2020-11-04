using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.PhilipsHue.Models;
using Artemis.UI.Shared.Services;
using MaterialDesignThemes.Wpf;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using Stylet;

namespace Artemis.Plugins.PhilipsHue.ViewModels
{
    public class PhilipsHueConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly PluginSetting<int> _pollingRateSetting;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly PluginSetting<List<PhilipsHueBridge>> _storedBridgesSetting;
        private bool _foundNewBridge;
        private string _lightDisplay;
        private bool _locatingBridges;
        private PhilipsHueBridge _newBridge;
        private int _pollingRate;
        private string _roomDisplay;
        private int _wizardPage;


        public PhilipsHueConfigurationViewModel(Plugin plugin,
            PluginSettings settings,
            ISnackbarMessageQueue snackbarMessageQueue,
            IDialogService dialogService,
            IModelValidator<PhilipsHueConfigurationViewModel> validator) : base(plugin, validator)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            _dialogService = dialogService;

            _storedBridgesSetting = settings.GetSetting("Bridges", new List<PhilipsHueBridge>());
            _pollingRateSetting = settings.GetSetting("PollingRate", 2);

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

        #region Settings

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
        }

        #endregion

        #region Bridge discovery
        
        public async Task FindHueBridge()
        {
            LocatingBridges = true;
            IBridgeLocator locator = new HttpBridgeLocator();
            List<LocatedBridge> bridges = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).ToList();
            
            // Lets try to find some more 
            locator = new SsdpBridgeLocator();
            IEnumerable<LocatedBridge> extraBridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            foreach (LocatedBridge extraBridge in extraBridges)
            {
                if (bridges.All(b => b.BridgeId != extraBridge.BridgeId))
                    bridges.Add(extraBridge);
            }

            await Task.Delay(1000);

            LocatedBridge newBridge = bridges.FirstOrDefault(b => _storedBridgesSetting.Value.All(s => s.BridgeId != b.BridgeId));
            if (newBridge == null)
            {
                FoundNewBridge = false;
                _newBridge = null;
                return;
            }

            FoundNewBridge = true;
            _newBridge = new PhilipsHueBridge(newBridge);

            LocatingBridges = false;

            if (FoundNewBridge)
                RegisterHueBridge();
        }

        private async Task RegisterHueBridge()
        {
            WizardPage = 1;

            ILocalHueClient client = new LocalHueClient(_newBridge.IpAddress);
            string registrationResult = null;
            int attempts = 0;

            void OnClosed(object? sender, CloseEventArgs args)
            {
                attempts = 12;
            }

            Closed += OnClosed;
            while (registrationResult == null && attempts < 12)
            {
                try
                {
                    registrationResult = await client.RegisterAsync("artemis-hue-plugin", Environment.MachineName);
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

            _newBridge.AppKey = registrationResult;
            _storedBridgesSetting.Value.Add(_newBridge);
            _storedBridgesSetting.Save();

            FinishWizard();
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