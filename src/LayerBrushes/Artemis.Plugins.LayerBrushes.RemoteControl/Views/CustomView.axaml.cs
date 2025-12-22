using Artemis.Plugins.LayerBrushes.RemoteControl.ViewModels;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Views;

public partial class CustomView : ReactiveUserControl<CustomViewModel>
{
    public CustomView()
    {
        InitializeComponent();
    }

}