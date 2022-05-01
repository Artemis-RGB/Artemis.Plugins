using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureScreenView : ReactiveUserControl<CaptureScreenViewModel>
{
    private readonly Image _displayPreviewImage;

    public CaptureScreenView()
    {
        InitializeComponent();
        _displayPreviewImage = this.Get<Image>("DisplayPreviewImage");

        this.WhenActivated(_ =>
        {
            if (ViewModel != null)
                ViewModel.PreviewImage = _displayPreviewImage;
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}