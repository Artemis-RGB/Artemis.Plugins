using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public partial class CaptureRegionDisplayView : ReactiveUserControl<CaptureRegionDisplayViewModel>
{
    private readonly Image _displayPreviewImage;

    public CaptureRegionDisplayView()
    {
        InitializeComponent();
        _displayPreviewImage = this.Get<Image>("DisplayPreviewImage");
        this.WhenActivated(d => { ViewModel!.PreviewImage = _displayPreviewImage; });
    }
}