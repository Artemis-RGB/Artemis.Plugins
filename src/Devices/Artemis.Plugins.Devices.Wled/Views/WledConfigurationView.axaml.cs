using Artemis.Plugins.Devices.Wled.ViewModels;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.Wled.Views;

public partial class WledConfigurationView : ReactiveUserControl<WledConfigurationViewModel>
{
    public WledConfigurationView()
    {
        InitializeComponent();
    }
}