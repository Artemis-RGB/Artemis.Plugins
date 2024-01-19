using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.Devices.Wled.Settings;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Artemis.Plugins.Devices.Wled.ViewModels.Dialogs;

public class DeviceConfigurationDialogViewModel : DialogViewModelBase<DeviceDialogResult>
{
    #region Properties & Fields

    private readonly DeviceDefinition _device;
    private readonly PluginSettings _settings;
    private readonly IWindowService _windowService;
    private bool _hasChanges;

    public DeviceDefinition Device { get; }

    private string _hostname;
    public string Hostname
    {
        get => _hostname;
        set => RaiseAndSetIfChanged(ref _hostname, value);
    }

    private string _manufacturer;
    public string Manufacturer
    {
        get => _manufacturer;
        set => RaiseAndSetIfChanged(ref _manufacturer, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => RaiseAndSetIfChanged(ref _model, value);
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public ReactiveCommand<Unit, Unit> RemoveDevice { get; }

    #endregion

    #region Constructors

    public DeviceConfigurationDialogViewModel(DeviceDefinition device, PluginSettings settings, IWindowService windowService)
    {
        this._device = device;
        this._settings = settings;
        this._windowService = windowService;

        _hostname = _device.Hostname;
        _manufacturer = _device.Manufacturer;
        _model = _device.Model;

        this.ValidationRule(vm => vm.Hostname, v => !string.IsNullOrWhiteSpace(v), "A hostname is required");

        Save = ReactiveCommand.Create(ExecuteSave, ValidationContext.Valid);
        Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
        RemoveDevice = ReactiveCommand.CreateFromTask(ExecuteRemoveDevice);

        PropertyChanged += (_, _) => _hasChanges = true;
    }

    #endregion

    #region Methods

    private void ExecuteSave()
    {
        if (HasErrors)
            return;

        _device.Hostname = Hostname;
        _device.Manufacturer = Manufacturer;
        _device.Model = Model;

        Close(DeviceDialogResult.Save);
    }

    private async Task ExecuteCancel()
    {
        if (_hasChanges)
        {
            bool confirmed = await _windowService.ShowConfirmContentDialog("Discard changes", "Are you sure you want to discard your changes?");
            if (confirmed)
                Close(DeviceDialogResult.Cancel);
        }
        else
            Close(DeviceDialogResult.Cancel);
    }

    private async Task ExecuteRemoveDevice()
    {
        bool confirmed = await _windowService.ShowConfirmContentDialog("Remove device", "Are you sure you want to remove this device?");
        if (!confirmed)
            return;

        PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
        definitions.Value.Remove(_device);
        Close(DeviceDialogResult.Remove);
    }
}

#endregion

public enum DeviceDialogResult
{
    Save,
    Cancel,
    Remove
}