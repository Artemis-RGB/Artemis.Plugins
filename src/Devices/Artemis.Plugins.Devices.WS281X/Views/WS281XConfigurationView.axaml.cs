using Artemis.Plugins.Devices.WS281X.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.WS281X.Views;

public partial class WS281XConfigurationView : ReactiveUserControl<WS281XConfigurationViewModel>
{
    public WS281XConfigurationView()
    {
        InitializeComponent();
    }

}