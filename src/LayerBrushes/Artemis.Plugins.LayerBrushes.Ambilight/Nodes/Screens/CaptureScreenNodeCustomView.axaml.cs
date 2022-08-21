using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Nodes.Screens;

public partial class CaptureScreenNodeCustomView : ReactiveUserControl<CaptureScreenNodeCustomViewModel>
{
    public CaptureScreenNodeCustomView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}