using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public partial class CapturePropertiesView : ReactiveUserControl<CapturePropertiesViewModel>
{
    public CapturePropertiesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}