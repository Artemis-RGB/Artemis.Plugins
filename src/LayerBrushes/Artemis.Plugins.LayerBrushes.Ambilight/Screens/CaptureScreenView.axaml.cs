using ReactiveUI;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public partial class CaptureScreenView : ReactiveUserControl<CaptureScreenViewModel>
{
    public CaptureScreenView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (ViewModel != null)
                ViewModel.PreviewImage = DisplayPreviewImage;
        });
    }
}