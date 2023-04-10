using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

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