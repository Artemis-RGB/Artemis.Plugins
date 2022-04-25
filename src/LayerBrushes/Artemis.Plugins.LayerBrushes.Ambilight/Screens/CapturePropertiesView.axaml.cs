using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI;

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