using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.WS281X.Views.Dialogs;

public partial class DeviceConfigurationDialogView : ReactiveUserControl<DeviceConfigurationDialogViewModel>
{
    public DeviceConfigurationDialogView()
    {
        InitializeComponent();
    }
}