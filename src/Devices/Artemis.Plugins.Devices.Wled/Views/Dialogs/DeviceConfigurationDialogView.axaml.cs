using Artemis.Plugins.Devices.Wled.ViewModels.Dialogs;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Wled.Views.Dialogs;

public partial class DeviceConfigurationDialogView : ReactiveAppWindow<DeviceConfigurationDialogViewModel>
{
    public DeviceConfigurationDialogView()
    {
        InitializeComponent();
    }
}