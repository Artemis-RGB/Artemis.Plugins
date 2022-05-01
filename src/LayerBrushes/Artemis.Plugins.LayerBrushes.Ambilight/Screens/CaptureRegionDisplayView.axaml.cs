using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public partial class CaptureRegionDisplayView : UserControl
{
    public CaptureRegionDisplayView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}