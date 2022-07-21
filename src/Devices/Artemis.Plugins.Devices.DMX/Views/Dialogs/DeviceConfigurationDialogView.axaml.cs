using Artemis.Plugins.Devices.DMX.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Devices.DMX.Views.Dialogs;

public partial class DeviceConfigurationDialogView : ReactiveCoreWindow<DeviceConfigurationDialogViewModel>
{
    public DeviceConfigurationDialogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}