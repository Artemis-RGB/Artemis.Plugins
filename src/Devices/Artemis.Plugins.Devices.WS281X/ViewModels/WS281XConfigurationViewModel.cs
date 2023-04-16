using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ContentDialogButton = Artemis.UI.Shared.Services.Builders.ContentDialogButton;

namespace Artemis.Plugins.Devices.WS281X.ViewModels;

public class WS281XConfigurationViewModel : PluginConfigurationViewModel
{
    private readonly PluginSetting<List<DeviceDefinition>> _definitions;
    private readonly IPluginManagementService _pluginManagementService;
    private readonly PluginSettings _settings;
    private readonly IWindowService _windowService;


    public WS281XConfigurationViewModel(Plugin plugin, PluginSettings settings, IWindowService windowService, IPluginManagementService pluginManagementService) : base(plugin)
    {
        _settings = settings;
        _windowService = windowService;
        _pluginManagementService = pluginManagementService;
        _definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());

        //DeviceDefinitions setting may be loaded with a null list
        if (_definitions.Value == null)
            _definitions.Value = new List<DeviceDefinition>();
            
        Definitions = new ObservableCollection<DeviceDefinition>(_definitions.Value);
        TurnOffLedsOnShutdown = _settings.GetSetting("TurnOffLedsOnShutdown", false);

        AddDevice = ReactiveCommand.CreateFromTask(ExecuteAddDevice);
        EditDevice = ReactiveCommand.CreateFromTask<DeviceDefinition>(ExecuteEditDevice);
        RemoveDevice = ReactiveCommand.Create<DeviceDefinition>(ExecuteRemoveDevice);
        Save = ReactiveCommand.Create(ExecuteSave);
        Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
    }

    public ReactiveCommand<Unit, Unit> AddDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> EditDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> RemoveDevice { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }

    public ObservableCollection<DeviceDefinition> Definitions { get; }
    public PluginSetting<bool> TurnOffLedsOnShutdown { get; }

    private async Task ExecuteAddDevice()
    {
        DeviceDefinition device = new() {Name = $"Device {_definitions.Value.Count + 1}"};
        ContentDialogResult result = await _windowService.CreateContentDialog()
            .WithTitle("Add device")
            .WithViewModel(out DeviceConfigurationDialogViewModel vm, device)
            .HavingPrimaryButton(b => b.WithText("Accept").WithCommand(vm.Accept))
            .WithDefaultButton(ContentDialogButton.Primary)
            .ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            _definitions.Value.Add(device);
            Definitions.Add(device);
        }
    }

    private async Task ExecuteEditDevice(DeviceDefinition device)
    {
        await _windowService.CreateContentDialog()
            .WithTitle("Edit device")
            .WithViewModel(out DeviceConfigurationDialogViewModel vm, device)
            .HavingPrimaryButton(b => b.WithText("Accept").WithCommand(vm.Accept))
            .HavingSecondaryButton(b => b.WithText("Delete").WithAction(() => ExecuteRemoveDevice(device)))
            .WithDefaultButton(ContentDialogButton.Primary)
            .ShowAsync();
    }

    private void ExecuteRemoveDevice(DeviceDefinition device)
    {
        _definitions.Value.Remove(device);
        Definitions.Remove(device);
    }

    private void ExecuteSave()
    {
        _definitions.Save();
        TurnOffLedsOnShutdown.Save();

        // Fire & forget re-enabling the plugin
        Task.Run(() =>
        {
            WS281XDeviceProvider deviceProvider = Plugin.GetFeature<WS281XDeviceProvider>();
            if (deviceProvider == null || !deviceProvider.IsEnabled) return;
            _pluginManagementService.DisablePluginFeature(deviceProvider, false);
            _pluginManagementService.EnablePluginFeature(deviceProvider, false);
        });

        Close();
    }

    private async Task ExecuteCancel()
    {
        if (TurnOffLedsOnShutdown.HasChanged || _definitions.HasChanged)
        {
            if (!await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                return;
        }

        _definitions.RejectChanges();
        _settings.GetSetting("TurnOffLedsOnShutdown", false).RejectChanges();

        Close();
    }
}