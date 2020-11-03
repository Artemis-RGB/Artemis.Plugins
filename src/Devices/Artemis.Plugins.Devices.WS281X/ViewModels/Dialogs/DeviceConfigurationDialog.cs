using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs
{
    public class DeviceConfigurationDialog : DialogViewModelBase
    {
        public DeviceConfigurationDialog()
        {
            Device = new DeviceDefinition();
        }

        public DeviceConfigurationDialog(DeviceDefinition device)
        {
            Device = device;
        }

        public DeviceDefinition Device { get; }
    }
}