using System;
using System.Runtime.InteropServices;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public sealed class DisplayPreview : ReactiveObject, IDisposable
{
    #region Properties & Fields

    private bool _isDisposed;

    private readonly CaptureZone _captureZone;
    private readonly CaptureZone? _processedCaptureZone;

    private readonly bool _blackBarDetectionTop;
    private readonly bool _blackBarDetectionBottom;
    private readonly bool _blackBarDetectionLeft;
    private readonly bool _blackBarDetectionRight;

    private readonly byte[] _previewBuffer;
    private readonly byte[]? _processedPreviewBuffer;

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
        _previewBuffer = new byte[_captureZone.Buffer.Length];
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
        _previewBuffer = new byte[_captureZone.Buffer.Length];
        Preview = new WriteableBitmap(new PixelSize(_captureZone.Width, _captureZone.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);

        if (properties.X + properties.Width <= display.Width && properties.Y + properties.Height <= display.Height)
        {
            _processedCaptureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display)
                .RegisterCaptureZone(properties.X, properties.Y, properties.Width, properties.Height, properties.DownscaleLevel);
            _processedCaptureZone.BlackBars.Threshold = properties.BlackBarDetectionThreshold;
            _processedPreviewBuffer = new byte[_processedCaptureZone.Buffer.Length];
            ProcessedPreview = new WriteableBitmap(
                new PixelSize(_processedCaptureZone.Width, _processedCaptureZone.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque
            );
        }
    }

    #endregion

    #region Methods

    public void Update()
    {
        if (_isDisposed) return;

        lock (_captureZone.Buffer)
        {
            WritePixels(Preview, _captureZone);
        }

        if (_processedCaptureZone == null)
            return;

        lock (_processedCaptureZone.Buffer)
        {
            if (_processedCaptureZone.Buffer.Length == 0)
                return;
            if (_blackBarDetectionTop || _blackBarDetectionBottom || _blackBarDetectionLeft || _blackBarDetectionRight)
            {
                int x = _blackBarDetectionLeft ? _processedCaptureZone.BlackBars.Left : 0;
                int y = _blackBarDetectionTop ? _processedCaptureZone.BlackBars.Top : 0;
                int width = _processedCaptureZone.Width - (_blackBarDetectionRight ? _processedCaptureZone.BlackBars.Right : 0) - x;
                int height = _processedCaptureZone.Height - (_blackBarDetectionBottom ? _processedCaptureZone.BlackBars.Bottom : 0) - y;
                if (width <= 0 && height <= 0)
                    return;

                if (ProcessedPreview == null || Math.Abs(ProcessedPreview.Size.Width - width) > 0.001 || Math.Abs(ProcessedPreview.Size.Height - height) > 0.001)
                    ProcessedPreview = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
                WriteCroppedPixels(ProcessedPreview, _processedCaptureZone);
            }
            else if (ProcessedPreview != null)
            {
                WritePixels(ProcessedPreview, _processedCaptureZone);
            }
        }
    }

    private void WritePixels(WriteableBitmap preview, CaptureZone captureZone)
    {
        using ILockedFramebuffer framebuffer = preview.Lock();
        Marshal.Copy(captureZone.Buffer, 0, framebuffer.Address, captureZone.Buffer.Length);
    }

    private void WriteCroppedPixels(WriteableBitmap preview, CaptureZone captureZone)
    {
        using ILockedFramebuffer framebuffer = preview.Lock();
        IntPtr framebufferPtr = framebuffer.Address;
        
        int left = _blackBarDetectionLeft ? captureZone.BlackBars.Left : 0;
        int right = _blackBarDetectionRight ? captureZone.BlackBars.Right : 0;
        int top = _blackBarDetectionTop ? captureZone.BlackBars.Top : 0;
        int bottom = _blackBarDetectionBottom ? captureZone.BlackBars.Bottom : 0;
        
        int stride = captureZone.Stride;
        int rowOffset = left * captureZone.BytesPerPixel;
        int rowLength = (captureZone.Width - left - right) * captureZone.BytesPerPixel;
        int height = captureZone.Height;
        
        for (int y = top; y < (height - bottom); y++)
        {
            int offset = (y * stride) + rowOffset;
            Marshal.Copy(captureZone.Buffer, offset, framebufferPtr, rowLength);
            framebufferPtr += rowLength;
        }
    }

    public void Dispose()
    {
        AmbilightBootstrapper.ScreenCaptureService!.GetScreenCapture(Display).UnregisterCaptureZone(_captureZone);
        _isDisposed = true;
    }

    #endregion
}