using System;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeypressRipple : IKeyPressEffect
    {
        private readonly KeypressBrush _brush;
        private float _progress;
        private SKColor _trailColor;
        private SKPaint _trailPaint;

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
                        _brush.Properties.RippleWidth,
                        _brush.Properties.Colors.BaseValue.GetColorsArray(),
                        _brush.Properties.Colors.BaseValue.GetPositionsArray(),
                        // Changed from Clamp to repeat. It just looks a lot better this way.
                        // Repeat will need a color position calculation by the way to get the inner ripple color ir order to paint the Trail.
                        SKShaderTileMode.Repeat
                    )
                };
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.ColorChange)
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = _brush.Properties.Colors.CurrentValue.GetColor(_progress)};
            }

            byte alpha = 255;
            // Add fade away effect
            if (_brush.Properties.RippleFadeAway != RippleFadeOutMode.None)
                alpha = (byte) (255d * Easings.Interpolate(1f - _progress, (Easings.Functions) _brush.Properties.RippleFadeAway.CurrentValue));

            // If we have to paint a trail
            if (_brush.Properties.RippleTrail)
            {
                // Moved trail color calculation here to avoid extra overhead when trail is not enabled
                _trailColor = _brush.Properties.ColorMode.CurrentValue switch
                {
                    // If gradient is used, calculate the inner color to a given position.
                    ColorType.Gradient => _brush.Properties.Colors.CurrentValue.GetColor((Size - _brush.Properties.RippleWidth / 2f) % _brush.Properties.RippleWidth / _brush.Properties.RippleWidth),
                    // If not gradient, we can just copy the color of the ripple Paint.
                    _ => Paint.Color
                };

                // Dispose before to create a new one. Thanks for the lesson.
                _trailPaint?.Dispose();
                _trailPaint = new SKPaint
                {
                    Shader = SKShader.CreateRadialGradient(
                        Position,
                        Size,
                        // Trail is simply a gradient from full inner ripple color to the same color but with alpha 0. Just an illution :D
                        new[] {_trailColor.WithAlpha(0), _trailColor.WithAlpha(alpha)},
                        new[] {0f, 1f},
                        SKShaderTileMode.Clamp
                    )
                };
                _trailPaint.Style = SKPaintStyle.Fill;
            }

            // Set ripple size and final color alpha
            Paint.Color = Paint.Color.WithAlpha(alpha);
            Paint.Style = SKPaintStyle.Stroke;
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
        public bool Finished => Size < 0f;
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

            // Draw the trail
            if (_brush.Properties.RippleTrail)
                // Start from end of ripple circle and ensure radios is never 0
                canvas.DrawCircle(Position, Math.Max(0, Size - _brush.Properties.RippleWidth.CurrentValue / 2f), _trailPaint);
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