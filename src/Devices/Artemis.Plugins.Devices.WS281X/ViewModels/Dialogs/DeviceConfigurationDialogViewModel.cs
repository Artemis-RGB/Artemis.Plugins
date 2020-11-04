using System.IO.Ports;
using System.Threading.Tasks;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using FluentValidation;
using Stylet;

namespace Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs
{
    public class DeviceConfigurationDialogViewModel : DialogViewModelBase
    {
        private readonly DeviceDefinition _device;
        private string _name;
        private string _port;
        private DeviceDefinitionType _type;

        public DeviceConfigurationDialogViewModel(DeviceDefinition device, IModelValidator<DeviceConfigurationDialogViewModel> validator) : base(validator)
        {
            _device = device;

            Name = _device.Name;
            Type = _device.Type;
            Port = _device.Port;

            Ports = new BindableCollection<string>(SerialPort.GetPortNames());
            DeviceTypes = new BindableCollection<ValueDescription>(EnumUtilities.GetAllValuesAndDescriptions(typeof(DeviceDefinitionType)));
        }

        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        public DeviceDefinitionType Type
        {
            get => _type;
            set => SetAndNotify(ref _type, value);
        }

        public string Port
        {
            get => _port;
            set => SetAndNotify(ref _port, value);
        }

        public BindableCollection<string> Ports { get; }
        public BindableCollection<ValueDescription> DeviceTypes { get; }

        public async Task Accept()
        {
            await ValidateAsync();

            if (HasErrors)
                return;

            if (!string.IsNullOrWhiteSpace(Name))
                _device.Name = Name;
            _device.Type = Type;
            _device.Port = Port;
            Session.Close(_device);
        }

        public void Cancel()
        {
            Session.Close();
        }
    }

    public class DeviceConfigurationDialogViewModelValidator : AbstractValidator<DeviceConfigurationDialogViewModel>
    {
        public DeviceConfigurationDialogViewModelValidator()
        {
            RuleFor(m => m.Port).NotEmpty().WithMessage("Device port is required");
            RuleFor(m => m.Type).NotNull().WithMessage("Device type is required");
        }
    }
}