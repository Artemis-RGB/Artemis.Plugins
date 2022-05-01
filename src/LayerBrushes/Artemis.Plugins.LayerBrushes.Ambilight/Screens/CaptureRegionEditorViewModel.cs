using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.UI.Shared;
using Avalonia;
using ReactiveUI;
using ScreenCapture.NET;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureRegionEditorViewModel : ActivatableViewModelBase
{
    private Rect _captureRegion;

    public CaptureRegionEditorViewModel(CapturePropertiesViewModel capturePropertiesViewModel, Display display)
    {
        CapturePropertiesViewModel = capturePropertiesViewModel;
        Display = display;

        this.WhenActivated(d =>
        {
            CapturePropertiesViewModel.WhenAnyValue(vm => vm.X, vm => vm.Y, vm => vm.Width, vm => vm.Height)
                .Subscribe(_ => CaptureRegionUpdated())
                .DisposeWith(d);
        });
    }

    public CapturePropertiesViewModel CapturePropertiesViewModel { get; }
    public Display Display { get; }

    public Rect CaptureRegion
    {
        get => _captureRegion;
        private set => RaiseAndSetIfChanged(ref _captureRegion, value);
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