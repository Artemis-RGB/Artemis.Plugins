using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeypressRipple : IKeyPressEffect
    {
        private readonly KeypressBrush _brush;
        private float _progress;

        public KeypressRipple(KeypressBrush brush, ArtemisLed led, SKPoint position)
        {
            _brush = brush;
            Led = led;
            Position = position;
            Expand = true;
            UpdatePaint();
        }

        public SKPaint Paint { get; set; }
        public float Size { get; set; }
        public bool Expand { get; set; }

        public void UpdateOne(double deltaTime)
        {
            if (Expand)
                Size += (float) (deltaTime * _brush.Properties.RippleGrowthSpeed.CurrentValue);
            else
                Size = -1;

            if (Size > _brush.Properties.RippleSize) Expand = false;

            UpdatePaint();
        }

        private void UpdatePaint()
        {
            if (_brush.Properties.ColorMode.CurrentValue == ColorType.Random && Paint == null)
            {
                Paint = new SKPaint {Color = SKColor.FromHsv(_brush.Rand.Next(0, 360), 100, 100)};
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Solid)
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = _brush.Properties.Color.CurrentValue};
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Gradient)
            {
                Paint?.Dispose();
                Paint = new SKPaint
                {
                    Shader = SKShader.CreateRadialGradient(
                        Position,
                        Size,
                        _brush.Properties.Colors.BaseValue.GetColorsArray(),
                        _brush.Properties.Colors.BaseValue.GetPositionsArray(),
                        SKShaderTileMode.Clamp
                    )
                };
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.ColorChange)
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = _brush.Properties.Colors.CurrentValue.GetColor(_progress)};
            }

            // Add fade away effect
            if (_brush.Properties.RippleFadeAway != RippleFadeOutMode.None)
                Paint.Color = Paint.Color.WithAlpha((byte) (255 * Easings.Interpolate(1 - _progress, (Easings.Functions) _brush.Properties.RippleFadeAway.BaseValue)));

            // Set ripple size
            Paint.Style = SKPaintStyle.Stroke;
            Paint.IsAntialias = true;
            Paint.StrokeWidth = _brush.Properties.RippleWidth.CurrentValue;
        }

        private void UpdateContinuous(double deltaTime)
        {
            if (Expand)
                Size += (float) (deltaTime * _brush.Properties.RippleGrowthSpeed.CurrentValue);
            else
                Size = -1;

            if (Size > _brush.Properties.RippleSize) Size = 0;

            UpdatePaint();
        }

        public bool AllowDuplicates => _brush.Properties.RippleBehivor == RippleBehivor.CreateNewRipple;
        public bool Finished => Size < 0;
        public ArtemisLed Led { get; }
        public SKPoint Position { get; set; }

        public void Update(double deltaTime)
        {
            if (_brush.Properties.RippleBehivor.CurrentValue == RippleBehivor.ContinuousWhileKeyPressed)
                UpdateContinuous(deltaTime);
            else
                UpdateOne(deltaTime);

            _progress = Size / _brush.Properties.RippleSize;
        }

        public void Render(SKCanvas canvas)
        {
            // Animation finished. Nothing to see here.
            if (Size < 0)
                return;

            if (Size > 0 && Paint != null)
                canvas.DrawCircle(Position, Size, Paint);
        }

        public void Respawn()
        {
            Expand = true;
            if (_brush.Properties.RippleBehivor == RippleBehivor.ResetCurrentRipple)
                Size = 0;
        }

        public void Despawn()
        {
            if (_brush.Properties.RippleBehivor.CurrentValue == RippleBehivor.ContinuousWhileKeyPressed)
                Expand = false;
        }
    }
}