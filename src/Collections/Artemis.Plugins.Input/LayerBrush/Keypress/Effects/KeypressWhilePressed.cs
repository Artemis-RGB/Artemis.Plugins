using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeypressWhilePressed : IKeyPressEffect
    {
        private readonly KeypressBrush _brush;

        public KeypressWhilePressed(KeypressBrush brush, ArtemisLed led, SKPoint position)
        {
            _brush = brush;
            Led = led;
            Position = position;

            UpdatePaint();
        }

        public SKPaint Paint { get; set; }
        public float Size { get; set; }
        public float GrowthSpeed { get; set; } = 500;
        public float MaxSize { get; set; } = 100;
        public bool Shrink { get; set; }
        public bool AllowDuplicates => false;
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
            if (!Shrink)
                Size += (float) (deltaTime * GrowthSpeed);
            else
                Size -= (float) (deltaTime * GrowthSpeed);

            if (Size > MaxSize)
                Size = MaxSize;

            UpdatePaint();
        }

        public void Render(SKCanvas canvas)
        {
            if (Size > 0 && Paint != null)
                canvas.DrawCircle(Position, Size, Paint);
        }

        public void Respawn()
        {
            Shrink = false;
        }

        public void Despawn()
        {
            Shrink = true;
        }
    }
}