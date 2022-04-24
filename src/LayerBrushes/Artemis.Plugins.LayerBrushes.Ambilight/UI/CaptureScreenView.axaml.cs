using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI;

public partial class CaptureScreenView : ReactiveUserControl<CaptureScreenViewModel>
{
    public CaptureScreenView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}