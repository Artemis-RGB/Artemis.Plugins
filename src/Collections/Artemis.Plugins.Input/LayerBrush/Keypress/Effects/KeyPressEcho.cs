using System;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeyPressEcho : IKeyPressEffect
    {
        private KeypressBrush _brush;

        public bool Finished => Progress >= 1.0;
        public bool AllowDuplicates => false;

        public ArtemisLed Led { get; }
        public SKPoint Position { get; set; }
        public SKPaint Paint { get; set; }
        public double Progress { get; set; }
        public bool Fade { get; set; }

        public KeyPressEcho(KeypressBrush brush, ArtemisLed led, SKPoint position)
        {
            _brush = brush;
            Led = led;
            Position = position;

            UpdatePaint(false);
        }

        private void UpdatePaint(bool respawn)
        {
            if (_brush.Properties.ColorMode.CurrentValue == ColorType.Random && (Paint == null || respawn))
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = SKColor.FromHsv(_brush.Rand.Next(0, 360), 100, 100)};
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Solid)
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = _brush.Properties.Color.CurrentValue};
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Gradient || _brush.Properties.ColorMode.CurrentValue == ColorType.ColorChange)
            {
                Paint?.Dispose();
                Paint = new SKPaint {Color = _brush.Properties.Colors.CurrentValue.GetColor((float) Progress)};
            }
        }

        public void Update(double deltaTime)
        {
            if (Fade)
                Progress += deltaTime * (1f / _brush.Properties.EchoLifetime.CurrentValue);
            UpdatePaint(false);
        }

        public void Render(SKCanvas canvas)
        {
            SKRect rect = SKRect.Create(
                Led.AbsoluteRectangle.Left - _brush.Layer.Bounds.Left,
                Led.AbsoluteRectangle.Top - _brush.Layer.Bounds.Top,
                Led.AbsoluteRectangle.Width,
                Led.AbsoluteRectangle.Height
            );
            
            if (Fade)
            {
                if (Progress > 1)
                    return;
                Paint.Color = Paint.Color.WithAlpha((byte) (255 * (1.0f - Progress)));
            }

            canvas.DrawRect(rect, Paint);
        }

        public void Respawn()
        {
            Fade = false;
            Progress = 0;
            UpdatePaint(true);
        }

        public void Despawn()
        {
            Fade = true;
        }
    }
}