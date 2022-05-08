using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.PhilipsHue.Models;
using Artemis.Plugins.PhilipsHue.Services;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using FluentAvalonia.UI.Controls;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using ReactiveUI;
using ContentDialogButton = Artemis.UI.Shared.Services.Builders.ContentDialogButton;

namespace Artemis.Plugins.PhilipsHue.ViewModels;

public class PhilipsHueConfigurationViewModel : PluginConfigurationViewModel
{
    private readonly IHueService _hueService;
    private readonly PluginSetting<int> _pollingRateSetting;
    private readonly PluginSetting<List<PhilipsHueBridge>> _storedBridgesSetting;
    private readonly IWindowService _windowService;
    private readonly INotificationService _notificationService;
    private string _finishText;
    private bool _foundNewBridge;
    private string _lightDisplay;
    private bool _locatingBridges;
    private PhilipsHueBridge _newBridge;
    private int _pollingRate;
    private string _roomDisplay;
    private int _wizardPage;


    public PhilipsHueConfigurationViewModel(Plugin plugin, PluginSettings settings, IHueService hueService, IWindowService windowService, INotificationService notificationService) : base(plugin)
    {
        _hueService = hueService;
        _windowService = windowService;
        _notificationService = notificationService;

        _storedBridgesSetting = settings.GetSetting("Bridges", new List<PhilipsHueBridge>());
        _pollingRateSetting = settings.GetSetting("PollingRate", 2000);
        // Reset to default if the setting is below 100ms because the scale changed from seconds to milliseconds
        if (_pollingRateSetting.Value < 100)
            _pollingRateSetting.Value = 2000;

        PollingRate = _pollingRateSetting.Value;
        if (_storedBridgesSetting.Value.Any())
            WizardPage = 3;

        this.WhenAnyValue(vm => vm.PollingRate).Subscribe(SavePollingRate);

        FindHueBridge = ReactiveCommand.CreateFromTask(ExecuteFindHueBridge);
        FindHueBridgeManual = ReactiveCommand.CreateFromTask(ExecuteFindHueBridgeManual, this.WhenAnyValue(vm => vm.LocatingBridges).Select(l => !l));
        ShowSettings = ReactiveCommand.Create(() => WizardPage = 3);
        AddHueBridge = ReactiveCommand.Create(ExecuteAddHueBridge);
        ResetPlugin = ReactiveCommand.CreateFromTask(ExecuteResetPlugin);
        ReloadDeviceProvider = ReactiveCommand.Create(ExecuteReloadDeviceProvider);

        WizardPage = 1;
    }

    public ReactiveCommand<Unit, int> ShowSettings { get; }

    public int PollingRate
    {
        get => _pollingRate;
        set => RaiseAndSetIfChanged(ref _pollingRate, value);
    }

    public bool LocatingBridges
    {
        get => _locatingBridges;
        set => RaiseAndSetIfChanged(ref _locatingBridges, value);
    }

    public bool FoundNewBridge
    {
        get => _foundNewBridge;
        set => RaiseAndSetIfChanged(ref _foundNewBridge, value);
    }

    public int WizardPage
    {
        get => _wizardPage;
        set => RaiseAndSetIfChanged(ref _wizardPage, value);
    }

    public string FinishText
    {
        get => _finishText;
        set => RaiseAndSetIfChanged(ref _finishText, value);
    }

    #region Settings

    public ReactiveCommand<Unit, Unit> AddHueBridge { get; }
    public ReactiveCommand<Unit, Unit> ResetPlugin { get; }
    public ReactiveCommand<Unit, Unit> ReloadDeviceProvider { get; }

    private void ExecuteAddHueBridge()
    {
        WizardPage = 0;
    }

    private async Task ExecuteResetPlugin()
    {
        bool result = await _windowService.ShowConfirmContentDialog(
            "Reset Philips Hue Plugin",
            "Are you sure about this? You'll have to reconnect with your Hue Bridge to continue using the plugin."
        );
        if (!result)
            return;

        _storedBridgesSetting.Value.Clear();
        _storedBridgesSetting.Save();

        WizardPage = 0;
        ExecuteReloadDeviceProvider();
    }

    private void SavePollingRate(int pollingRate)
    {
        if (!ValidationContext.IsValid)
            return;

        _pollingRateSetting.Value = PollingRate;
        _pollingRateSetting.Save();
    }

    private void ExecuteReloadDeviceProvider()
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

    #endregion

    #region Bridge discovery

    public ReactiveCommand<Unit, Unit> FindHueBridge { get; }
    public ReactiveCommand<Unit, Unit> FindHueBridgeManual { get; }

    private async Task ExecuteFindHueBridge()
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
            _notificationService.CreateNotification().WithMessage("Could not find a (new) Hue Bridge.").Show();
            return;
        }

        FoundNewBridge = true;
        LocatingBridges = false;
        _newBridge = new PhilipsHueBridge(newBridge);

        if (FoundNewBridge)
            await RegisterHueBridge();
    }

    private async Task ExecuteFindHueBridgeManual()
    {
        if (LocatingBridges)
            return;

        ContentDialogResult result = await _windowService.CreateContentDialog()
            .WithTitle("Add Hue Bridge")
            .WithViewModel(out PhilipsHueBridgeDialogViewModel viewModel)
            .WithCloseButtonText("Cancel")
            .WithDefaultButton(ContentDialogButton.Primary)
            .HavingPrimaryButton(b => b.WithAction(viewModel.Accept).WithText("Accept"))
            .ShowAsync();
        if (result != ContentDialogResult.Primary)
            return;

        PhilipsHueBridge newBridge = new() {IpAddress = viewModel.IpAddress};
        if (_storedBridgesSetting.Value.Any(b => b.IpAddress == newBridge.IpAddress))
        {
            await ShowAddBridgeError($"A Hue Bridge with IP address {newBridge.IpAddress} is already added.");
            return;
        }

        LocatingBridges = true;

        using HttpClient httpClient = new();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"http://{newBridge.IpAddress}/description.xml");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            await ShowAddBridgeError($"Failed to connect to Hue Bridge at {newBridge.IpAddress}.");
            LocatingBridges = false;
            return;
        }

        string response = await httpResponseMessage.Content.ReadAsStringAsync();
        if (!response.Contains("philips hue bridge", StringComparison.InvariantCultureIgnoreCase))
        {
            await ShowAddBridgeError($"Device at {newBridge.IpAddress} does not appear to be a Hue Bridge.");
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

        void OnClosed(object sender, EventArgs eventArgs)
        {
            attempts = 12;
        }

        CloseRequested += OnClosed;

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

        CloseRequested += OnClosed;
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

        await FinishWizard();
        ExecuteReloadDeviceProvider();
    }

    private async Task FinishWizard()
    {
        WizardPage = 2;

        ILocalHueClient client = new LocalHueClient(_newBridge.IpAddress);
        client.Initialize(_newBridge.AppKey);

        IReadOnlyCollection<Group> groups = await client.GetGroupsAsync();
        IEnumerable<Light> lights = await client.GetLightsAsync();

        int roomCount = groups.Count(g => g.Type == GroupType.Room);
        int lightCount = lights.Count();

        FinishText = $"Artemis was able to access your Hue Bridge and is ready to expand the data model with info on your {lightCount} light(s) and {roomCount} room(s).";
        _newBridge = null;
    }

    private async Task ShowAddBridgeError(string error)
    {
        await _windowService.ShowConfirmContentDialog("Add Hue Bridge", error, "Close", null);
    }

    #endregion
}