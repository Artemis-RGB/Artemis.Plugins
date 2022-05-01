using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureRegionDisplayView : ReactiveUserControl<CaptureRegionDisplayViewModel>
{
    private readonly Image _displayPreviewImage;

    public CaptureRegionDisplayView()
    {
        InitializeComponent();
        _displayPreviewImage = this.Get<Image>("DisplayPreviewImage");
        this.WhenActivated(d => { ViewModel!.PreviewImage = _displayPreviewImage; });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}