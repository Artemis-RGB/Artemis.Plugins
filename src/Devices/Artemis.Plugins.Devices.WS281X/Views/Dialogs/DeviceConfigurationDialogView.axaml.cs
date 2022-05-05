using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.WS281X.Views.Dialogs;

public class DeviceConfigurationDialogView : ReactiveUserControl<DeviceConfigurationDialogViewModel>
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