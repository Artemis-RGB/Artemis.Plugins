using Artemis.Plugins.Devices.DMX.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.DMX.Views;

public partial class DMXConfigurationView : ReactiveUserControl<DMXConfigurationViewModel>
{
    public DMXConfigurationView()
    {
        InitializeComponent();
    }

}