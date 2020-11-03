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
        public DeviceConfigurationDialogViewModel(DeviceDefinition device, IModelValidator<DeviceConfigurationDialogViewModel> validator) : base(validator)
        {
            Device = device;
            DeviceTypes = new BindableCollection<ValueDescription>(EnumUtilities.GetAllValuesAndDescriptions(typeof(DeviceDefinitionType)));
        }

        public BindableCollection<ValueDescription> DeviceTypes { get; }
        public DeviceDefinition Device { get; }

        public async Task Accept()
        {
            await ValidateAsync();

            if (HasErrors)
                return;

            Session.Close(Device);
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
            RuleFor(m => m.Device.Port).NotEmpty().WithMessage("Device port is required");
            RuleFor(m => m.Device.Type).NotNull().WithMessage("Device type is required");
        }
    }
}