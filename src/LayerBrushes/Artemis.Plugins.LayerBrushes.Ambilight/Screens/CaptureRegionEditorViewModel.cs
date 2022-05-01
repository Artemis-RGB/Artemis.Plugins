using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Artemis.UI.Shared;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using ScreenCapture.NET;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureRegionEditorViewModel : ActivatableViewModelBase
{
    private Rect _captureRegion;
    private Image? _previewImage;

    public CaptureRegionEditorViewModel(CapturePropertiesViewModel capturePropertiesViewModel, Display display)
    {
        CapturePropertiesViewModel = capturePropertiesViewModel;
        Display = display;
        DisplayPreview = new DisplayPreview(Display);

        this.WhenActivated(d =>
        {
            CapturePropertiesViewModel
                .WhenAnyValue(vm => vm.X, vm => vm.Y, vm => vm.Width, vm => vm.Height)
                .Subscribe(_ => CaptureRegionUpdated())
                .DisposeWith(d);

            Disposable.Create(() => DisplayPreview.Dispose()).DisposeWith(d);
        });
    }

    public CapturePropertiesViewModel CapturePropertiesViewModel { get; }
    public Display Display { get; }
    public DisplayPreview DisplayPreview { get; }

    public Rect CaptureRegion
    {
        get => _captureRegion;
        private set => RaiseAndSetIfChanged(ref _captureRegion, value);
    }

    public Image? PreviewImage
    {
        get => _previewImage;
        set => RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public void Update()
    {
        DisplayPreview.Update();
        PreviewImage?.InvalidateVisual();
    }

    public SKRectI GetCaptureRegionRect()
    {
        return new SKRectI(
            CapturePropertiesViewModel.X,
            CapturePropertiesViewModel.Y,
            CapturePropertiesViewModel.X + CapturePropertiesViewModel.Width,
            CapturePropertiesViewModel.Y + CapturePropertiesViewModel.Height
        );
    }

    public void ApplyRegion()
    {
        CapturePropertiesViewModel.EnableValidation = true;
        Task.Run(CapturePropertiesViewModel.Save);
    }

    private void CaptureRegionUpdated()
    {
        CaptureRegion = new Rect(
            CapturePropertiesViewModel.X,
            CapturePropertiesViewModel.Y,
            CapturePropertiesViewModel.Width,
            CapturePropertiesViewModel.Height
        );
    }
}