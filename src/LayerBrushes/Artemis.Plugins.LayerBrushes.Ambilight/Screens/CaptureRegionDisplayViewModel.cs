using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.UI.Shared;
using Avalonia.Controls;
using ReactiveUI;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureRegionDisplayViewModel : ActivatableViewModelBase
{
    private Image? _previewImage;

    public CaptureRegionDisplayViewModel(Display display, AmbilightCaptureProperties properties)
    {
        Display = display;
        Properties = properties;
        DisplayPreview = new DisplayPreview(display, properties);
        this.WhenActivated(d => Disposable.Create(() => DisplayPreview.Dispose()).DisposeWith(d));
    }

    public Display Display { get; }
    public AmbilightCaptureProperties Properties { get; }
    public DisplayPreview DisplayPreview { get; }

    public Image? PreviewImage
    {
        get => _previewImage;
        set => RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public void Update()
    {
        DisplayPreview?.Update();
        PreviewImage?.InvalidateVisual();
    }
}