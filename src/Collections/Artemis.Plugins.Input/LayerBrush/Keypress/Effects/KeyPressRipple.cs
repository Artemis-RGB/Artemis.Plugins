using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeypressRipple : IKeyPressEffect
    {
        private readonly KeypressBrush _brush;

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
        public bool KeepExpanding { get; set; }
        public bool AllowDuplicates => _brush.Properties.RippleBehivor == RippleBehivor.CreateNewRipple;
        public bool Finished => Size < 0;
        public ArtemisLed Led { get; }
        public SKPoint Position { get; set; }

        private void UpdatePaint()
        {
            if (_brush.Properties.ColorMode.CurrentValue == ColorType.Random && Paint == null)
                Paint = new SKPaint { Color = SKColor.FromHsv(_brush.Rand.Next(0, 360), 100, 100) };
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Solid)
                Paint = new SKPaint { Color = _brush.Properties.Color.CurrentValue };
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Gradient)
            {
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
        }

        public void Update(double deltaTime)
        {
            if (_brush.Properties.RippleBehivor.CurrentValue == RippleBehivor.ContinuousWhileKeyPressed)
                UpdateContinuous(deltaTime);
            else
                UpdateOne(deltaTime);
        }

        public void UpdateOne(double deltaTime)
        {
            if (Expand)
                Size += (float)(deltaTime * _brush.Properties.RippleGrowthSpeed.CurrentValue);
            else
                Size = -1;

            if (Size > _brush.Properties.RippleSize)
            {
                Expand = false;
            }

            UpdatePaint();
        }

        private void UpdateContinuous(double deltaTime)
        {
            if (Expand)
                Size += (float)(deltaTime * _brush.Properties.RippleGrowthSpeed.CurrentValue);
            else
                Size = -1;

            if (Size > _brush.Properties.RippleSize)
            {
                Size = 0;
            }

            UpdatePaint();
        }

        public void Render(SKCanvas canvas)
        {
            //Animiation finished. Nothing to see here.
            if (Size < 0)
                return;

            //Just a check to avoid ripple goes beyond their desired size
            if (Size > _brush.Properties.RippleSize)
                Size = _brush.Properties.RippleSize;

            //Set ripple size.
            Paint.Style = SKPaintStyle.Stroke;
            Paint.IsAntialias = true;
            Paint.StrokeWidth = _brush.Properties.RippleWidth.CurrentValue;

            //Add fade away effect
            if (_brush.Properties.RippleFadeAway != RippleFadeOutMode.None)
            {
                Paint.Color = Paint.Color.WithAlpha((byte)(255 * Easings.Interpolate(1 - Size / _brush.Properties.RippleSize, (Easings.Functions)_brush.Properties.RippleFadeAway.BaseValue)));
            }

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