using System;
using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Ambilight.Nodes.Screens;
using Artemis.Plugins.LayerBrushes.Ambilight.Nodes.Storage;
using ScreenCapture.NET;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Nodes;

[Node("Capture Screen", "Captures a region of the screen", "Image", OutputType = typeof(SKBitmap))]
public class CaptureScreenNode : Node<CaptureScreenNodeStorage, CaptureScreenNodeCustomViewModel>
{
    #region Properties & Fields

    private CaptureZone? _captureZone;

    public OutputPin<SKBitmap> Output { get; set; }

    #endregion

    #region Constructors

    public CaptureScreenNode()
        : base("Capture Screen", "Captures a region of the screen")
    {
        Output = CreateOutputPin<SKBitmap>("Image");
        Storage = new CaptureScreenNodeStorage();
    }

    #endregion

    #region Metho

    public override unsafe void Evaluate()
    {
        //TODO DarthAffe 21.08.2022: Correctly implement
        if (_captureZone == null)
        {
            if (Output.Value?.DrawsNothing != true)
                Output.Value = new SKBitmap();

            return;
        }

        if (_captureZone.IsUpdateRequested) return;

        lock (_captureZone.Buffer)
        {
            ReadOnlySpan<byte> capture = _captureZone.Buffer;
            if (capture.IsEmpty) return;

            fixed (byte* ptr = capture)
                Output.Value = SKBitmap.FromImage(SKImage.FromPixels(new SKImageInfo(_captureZone.Width, _captureZone.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), new IntPtr(ptr), _captureZone.Stride));

            _captureZone.RequestUpdate();
        }
    }

    #endregion
}