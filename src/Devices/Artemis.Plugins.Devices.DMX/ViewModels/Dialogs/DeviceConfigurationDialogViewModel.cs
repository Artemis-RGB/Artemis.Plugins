using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.Devices.DMX.Settings;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.Builders;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.DMX.ViewModels.Dialogs
{
    public class DeviceConfigurationDialogViewModel : DialogViewModelBase<DeviceDialogResult>
    {
        private readonly DeviceDefinition _device;
        private readonly PluginSettings _settings;
        private readonly IWindowService _windowService;
        private string _hostname;
        private string _manufacturer;
        private string _model;
        private string _name;
        private int _port;
        private int _universe;
        private bool _hasChanges;

        public DeviceConfigurationDialogViewModel(DeviceDefinition device, PluginSettings settings, IWindowService windowService)
        {
            _device = device;
            _settings = settings;
            _windowService = windowService;

            _device = device;
            _name = _device.Name;
            _hostname = _device.Hostname;
            _port = _device.Port;
            _manufacturer = _device.Manufacturer;
            _model = _device.Model;
            _universe = _device.Universe;

            LedDefinitions = new ObservableCollection<LedDefinition>(device.LedDefinitions);

            this.ValidationRule(vm => vm.Hostname, v => !string.IsNullOrWhiteSpace(v), "A hostname is required");
            this.ValidationRule(vm => vm.Port, v => v > 0, "Device port is required");
            this.ValidationRule(vm => vm.Universe, v => v >= 1 && v <= 63999, "Universe must range from 1 to 63999");

            Save = ReactiveCommand.Create(ExecuteSave, ValidationContext.Valid);
            Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
            RemoveDevice = ReactiveCommand.CreateFromTask(ExecuteRemoveDevice);
            AddLed = ReactiveCommand.Create(ExecuteAddLed);
            AddLeds = ReactiveCommand.CreateFromTask(ExecuteAddLeds);
            ClearLeds = ReactiveCommand.CreateFromTask(ExecuteClearLeds);
            DeleteLed = ReactiveCommand.Create<LedDefinition>(ExecuteDeleteLed);

            PropertyChanged += (_, _) => _hasChanges = true;
        }

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, Unit> RemoveDevice { get; }
        public ReactiveCommand<Unit, Unit> AddLed { get; }
        public ReactiveCommand<Unit, Unit> AddLeds { get; }
        public ReactiveCommand<Unit, Unit> ClearLeds { get; }
        public ReactiveCommand<LedDefinition, Unit> DeleteLed { get; }

        public DeviceDefinition Device { get; }

        public ObservableCollection<LedDefinition> LedDefinitions { get; }

        public string Name
        {
            get => _name;
            set => RaiseAndSetIfChanged(ref _name, value);
        }

        public string Hostname
        {
            get => _hostname;
            set => RaiseAndSetIfChanged(ref _hostname, value);
        }

        public int Port
        {
            get => _port;
            set => RaiseAndSetIfChanged(ref _port, value);
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set => RaiseAndSetIfChanged(ref _manufacturer, value);
        }

        public string Model
        {
            get => _model;
            set => RaiseAndSetIfChanged(ref _model, value);
        }

        public int Universe
        {
            get => _universe;
            set => RaiseAndSetIfChanged(ref _universe, value);
        }

        private void ExecuteAddLed()
        {
            if (!LedDefinitions.Any())
            {
                LedDefinitions.Add(new LedDefinition
                {
                    R = 0,
                    G = 1,
                    B = 2
                });
            }
            else
            {
                LedDefinition previous = LedDefinitions.Last();
                LedDefinitions.Add(new LedDefinition
                {
                    R = previous.R + 3,
                    G = previous.G + 3,
                    B = previous.B + 3
                });
            }

            _hasChanges = true;
        }

        private async Task ExecuteAddLeds()
        {
            await _windowService.CreateContentDialog()
                .WithTitle("Add LEDs")
                .WithViewModel(out AddLedsDialogViewModel vm)
                .HavingPrimaryButton(b => b.WithText("Accept").WithAction(() =>
                {
                    for (int i = 0; i < vm.Amount; i++)
                        ExecuteAddLed();
                }))
                .WithDefaultButton(ContentDialogButton.Primary)
                .ShowAsync();
        }

        private void ExecuteDeleteLed(LedDefinition ledDefinition)
        {
            LedDefinitions.Remove(ledDefinition);

            _hasChanges = true;
        }

        private async Task ExecuteClearLeds()
        {
            bool confirmed = await _windowService.ShowConfirmContentDialog("Clear LEDs", "Are you sure you want to clear out all LEDs?");
            if (!confirmed)
                return;
            LedDefinitions.Clear();

            _hasChanges = true;
        }

        private void ExecuteSave()
        {
            if (HasErrors)
                return;

            _device.Name = Name;
            _device.Hostname = Hostname;
            _device.Port = Port;
            _device.Manufacturer = Manufacturer;
            _device.Model = Model;
            _device.Universe = (short) Universe;
            _device.LedDefinitions.Clear();
            _device.LedDefinitions.AddRange(LedDefinitions);

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

    public enum DeviceDialogResult
    {
        Save,
        Cancel,
        Remove
    }
}