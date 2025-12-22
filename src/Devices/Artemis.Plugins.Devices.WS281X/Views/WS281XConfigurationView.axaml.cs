using Artemis.Plugins.Devices.WS281X.ViewModels;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.WS281X.Views;

public partial class WS281XConfigurationView : ReactiveUserControl<WS281XConfigurationViewModel>
{
    public WS281XConfigurationView()
    {
        InitializeComponent();
    }

}