using System;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HPPH;
using ReactiveUI;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public sealed class DisplayPreview : ReactiveObject, IDisposable
{
    #region Properties & Fields

    private bool _isDisposed;

    private readonly ICaptureZone _captureZone;
    private readonly ICaptureZone? _processedCaptureZone;

    private readonly int _blackBarThreshold;
    private readonly bool _blackBarDetectionTop;
    private readonly bool _blackBarDetectionBottom;
    private readonly bool _blackBarDetectionLeft;
    private readonly bool _blackBarDetectionRight;

    public Display Display { get; }

    public WriteableBitmap Preview { get; }

    private WriteableBitmap? _processedPreview;
    public WriteableBitmap? ProcessedPreview
    {
        get => _processedPreview;
        set => this.RaiseAndSetIfChanged(ref _processedPreview, value);
    }

    #endregion

    #region Constructors

    public DisplayPreview(Display display, bool highQuality = false)
    {
        Display = display;

        _captureZone = AmbilightBootstrapper.ScreenCaptureService!.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height, highQuality ? 0 : 2);
        Preview = new WriteableBitmap(new PixelSize(_captureZone.Width, _captureZone.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
    }

    public DisplayPreview(Display display, AmbilightCaptureProperties properties)
    {
        Display = display;
        _blackBarDetectionBottom = properties.BlackBarDetectionBottom;
        _blackBarDetectionTop = properties.BlackBarDetectionTop;
        _blackBarDetectionLeft = properties.BlackBarDetectionLeft;
        _blackBarDetectionRight = properties.BlackBarDetectionRight;

        _captureZone = AmbilightBootstrapper.ScreenCaptureService!.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height);
        Preview = new WriteableBitmap(new PixelSize(_captureZone.Width, _captureZone.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);

        if (((properties.X + properties.Width) <= display.Width) && ((properties.Y + properties.Height) <= display.Height))
        {
            _processedCaptureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display)
                                                         .RegisterCaptureZone(properties.X, properties.Y, properties.Width, properties.Height, properties.DownscaleLevel);

            _blackBarThreshold = properties.BlackBarDetectionThreshold;
            ProcessedPreview = new WriteableBitmap(new PixelSize(_processedCaptureZone.Width, _processedCaptureZone.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        }
    }

    #endregion

    #region Methods

    public void Update()
    {
        if (_isDisposed) return;

        // DarthAffe 11.09.2023: Accessing the low-level images is a source of potential errors in the future since we assume the pixel-format. Currently both used providers are BGRA, but if there are ever issues with shifted colors, this is the place to start investigating.

        using (_captureZone.Lock())
            WritePixels(Preview, _captureZone.GetRefImage<ColorBGRA>());

        if (_processedCaptureZone == null)
            return;

        using (_processedCaptureZone.Lock())
        {
            if (_processedCaptureZone.RawBuffer.Length == 0)
                return;

            RefImage<ColorBGRA> processedImage = _processedCaptureZone.GetRefImage<ColorBGRA>();
            if (_blackBarDetectionTop || _blackBarDetectionBottom || _blackBarDetectionLeft || _blackBarDetectionRight)
            {
                RefImage<ColorBGRA> croppedImage = processedImage.RemoveBlackBars(_blackBarThreshold, _blackBarDetectionTop, _blackBarDetectionBottom, _blackBarDetectionLeft, _blackBarDetectionRight);

                if ((ProcessedPreview == null) || (Math.Abs(ProcessedPreview.Size.Width - croppedImage.Width) > 0.001) || (Math.Abs(ProcessedPreview.Size.Height - croppedImage.Height) > 0.001))
                    ProcessedPreview = new WriteableBitmap(new PixelSize(croppedImage.Width, croppedImage.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);

                WritePixels(ProcessedPreview, croppedImage);
            }
            else if (ProcessedPreview != null)
            {
                WritePixels(ProcessedPreview, processedImage);
            }
        }
    }

    private static unsafe void WritePixels(WriteableBitmap preview, RefImage<ColorBGRA> image)
    {
        using ILockedFramebuffer framebuffer = preview.Lock();
        image.CopyTo(new Span<ColorBGRA>((void*)framebuffer.Address, framebuffer.Size.Width * framebuffer.Size.Height));
    }

    public void Dispose()
    {
        AmbilightBootstrapper.ScreenCaptureService!.GetScreenCapture(Display).UnregisterCaptureZone(_captureZone);
        _isDisposed = true;
    }

    #endregion
}