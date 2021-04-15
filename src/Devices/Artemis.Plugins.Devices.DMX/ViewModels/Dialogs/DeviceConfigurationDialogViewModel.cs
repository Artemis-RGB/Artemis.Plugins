using System.Threading.Tasks;
using Artemis.Plugins.Devices.DMX.Settings;
using Artemis.UI.Shared.Services;
using FluentValidation;
using Stylet;

namespace Artemis.Plugins.Devices.DMX.ViewModels.Dialogs
{
    public class DeviceConfigurationDialogViewModel : DialogViewModelBase
    {
        private readonly DeviceDefinition _device;
        private string _hostname;
        private string _manufacturer;
        private string _model;
        private string _name;
        private int _port;
        private int _universe;

        public DeviceConfigurationDialogViewModel(DeviceDefinition device, IModelValidator<DeviceConfigurationDialogViewModel> validator) : base(validator)
        {
            _device = device;

            Name = _device.Name;
            Hostname = _device.Hostname;
            Port = _device.Port;
            Manufacturer = _device.Manufacturer;
            Model = _device.Model;
            Universe = _device.Universe;
        }

        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        public string Hostname
        {
            get => _hostname;
            set => SetAndNotify(ref _hostname, value);
        }

        public int Port
        {
            get => _port;
            set => SetAndNotify(ref _port, value);
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set => SetAndNotify(ref _manufacturer, value);
        }

        public string Model
        {
            get => _model;
            set => SetAndNotify(ref _model, value);
        }

        public int Universe
        {
            get => _universe;
            set => SetAndNotify(ref _universe, value);
        }

        public async Task Accept()
        {
            await ValidateAsync();

            if (HasErrors)
                return;

            _device.Name = Name;
            _device.Hostname = Hostname;
            _device.Port = Port;
            _device.Manufacturer = Manufacturer;
            _device.Model = Model;
            _device.Universe = (short) Universe;

            Session.Close(true);
        }
    }

    public class DeviceConfigurationDialogViewModelValidator : AbstractValidator<DeviceConfigurationDialogViewModel>
    {
        public DeviceConfigurationDialogViewModelValidator()
        {
            RuleFor(m => m.Hostname).NotEmpty().WithMessage("A hostname is required");
            RuleFor(m => m.Port).NotEmpty().WithMessage("Device port is required");
            RuleFor(m => m.Universe).GreaterThanOrEqualTo(1).WithMessage("Universe must range from 1 to 63999");
            RuleFor(m => m.Universe).LessThanOrEqualTo(63999).WithMessage("Universe must range from 1 to 63999");
        }
    }
}