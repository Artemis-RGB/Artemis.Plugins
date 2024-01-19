using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Wled.Settings;
using Artemis.Plugins.Devices.Wled.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using ReactiveUI;

namespace Artemis.Plugins.Devices.Wled.ViewModels;

public class WledConfigurationViewModel : PluginConfigurationViewModel
{
    #region Properties & Fields

    private readonly PluginSetting<List<DeviceDefinition>> _definitions;
    private readonly IPluginManagementService _pluginManagementService;
    private readonly PluginSettings _settings;
    private readonly IWindowService _windowService;

    public ObservableCollection<DeviceDefinition> DeviceDefinitions { get; }

    public PluginSetting<bool> EnableAutoDiscovery { get; }
    public PluginSetting<int> AutoDiscoveryTime { get; }
    public PluginSetting<int> AutoDiscoveryMaxDevices { get; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> AddDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> EditDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> RemoveDevice { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }

    #endregion

    #region Constructors

    public WledConfigurationViewModel(Plugin plugin, PluginSettings settings, IWindowService windowService, IPluginManagementService pluginManagementService)
        : base(plugin)
    {
        this._settings = settings;
        this._windowService = windowService;
        this._pluginManagementService = pluginManagementService;

        _definitions = _settings.GetSetting(nameof(DeviceDefinitions), new List<DeviceDefinition>());

        DeviceDefinitions = new ObservableCollection<DeviceDefinition>(_definitions.Value);
        EnableAutoDiscovery = _settings.GetSetting(nameof(EnableAutoDiscovery), false);
        AutoDiscoveryTime = _settings.GetSetting(nameof(AutoDiscoveryTime), 500);
        AutoDiscoveryMaxDevices = _settings.GetSetting(nameof(AutoDiscoveryMaxDevices), 0);

        AddDevice = ReactiveCommand.CreateFromTask(ExecuteAddDevice);
        EditDevice = ReactiveCommand.CreateFromTask<DeviceDefinition>(ExecuteEditDevice);
        RemoveDevice = ReactiveCommand.Create<DeviceDefinition>(ExecuteRemoveDevice);
        Save = ReactiveCommand.Create(ExecuteSave);
        Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
    }

    #endregion

    #region Methods

    private async Task ExecuteAddDevice()
    {
        DeviceDefinition device = new();
        if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) != DeviceDialogResult.Save)
            return;

        _definitions.Value.Add(device);
        DeviceDefinitions.Add(device);
    }

    private async Task ExecuteEditDevice(DeviceDefinition device)
    {
        if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) == DeviceDialogResult.Remove)
            DeviceDefinitions.Remove(device);
    }

    private async Task ExecuteCancel()
    {
        if (EnableAutoDiscovery.HasChanged
         || AutoDiscoveryTime.HasChanged
         || AutoDiscoveryMaxDevices.HasChanged
         || _definitions.HasChanged)
        {
            if (!await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                return;
        }

        _definitions.RejectChanges();
        _settings.GetSetting(nameof(EnableAutoDiscovery), false).RejectChanges();
        _settings.GetSetting(nameof(AutoDiscoveryTime), 500).RejectChanges();
        _settings.GetSetting(nameof(AutoDiscoveryMaxDevices), 0).RejectChanges();

        Close();
    }

    private void ExecuteRemoveDevice(DeviceDefinition device)
    {
        _definitions.Value.Remove(device);
        DeviceDefinitions.Remove(device);
    }

    private void ExecuteSave()
    {
        _definitions.Save();
        EnableAutoDiscovery.Save();
        AutoDiscoveryTime.Save();
        AutoDiscoveryMaxDevices.Save();

        // Fire & forget re-enabling the plugin
        Task.Run(() =>
                 {
                     WledDeviceProvider deviceProvider = Plugin.GetFeature<WledDeviceProvider>();
                     if ((deviceProvider == null) || !deviceProvider.IsEnabled) return;
                     _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                     _pluginManagementService.EnablePluginFeature(deviceProvider, false);
                 });

        Close();
    }

    #endregion
}