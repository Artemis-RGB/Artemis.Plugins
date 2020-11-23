using System;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress
{
    public class KeypressWave
    {
        public KeypressWave(ArtemisLed led, SKPoint position, SKPaint paint)
        {
            Led = led;
            Position = position;
            Paint = paint;
        }

        public ArtemisLed Led { get; }
        public SKPoint Position { get; set; }
        public SKPaint Paint { get; set; }

        public float Size { get; set; }
        public float GrowthSpeed { get; set; } = 500;
        public float MaxSize { get; set; } = 100;
        public bool Shrink { get; set; }

        public void Update(double deltaTime)
        {
            if (!Shrink)
                Size += (float) (deltaTime * GrowthSpeed);
            else
                Size -= (float) (deltaTime * GrowthSpeed);

            if (Size > MaxSize)
                Size = MaxSize;
        }

        public void Render(SKCanvas canvas)
        {
            if (Size > 0)
                canvas.DrawCircle(Position, Size, Paint);
        }
    }
}