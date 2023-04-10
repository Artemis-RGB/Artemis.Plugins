using Artemis.Plugins.LayerBrushes.RemoteControl.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Views;

public partial class CustomView : ReactiveUserControl<CustomViewModel>
{
    public CustomView()
    {
        InitializeComponent();
    }

}