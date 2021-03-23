using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes
{
    public class SKConfettiRectShape : SKConfettiShape
    {
        public SKConfettiRectShape()
        {
        }

        public SKConfettiRectShape(double heightRatio)
        {
            HeightRatio = heightRatio;
        }

        public double HeightRatio { get; set; } = 0.5;

        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float size)
        {
            float height = size * (float) HeightRatio;
            if (size <= 0 || height <= 0)
                return;

            SKRect rect = SKRect.Create(-size / 2f, -height / 2f, size, height);
            canvas.DrawRect(rect, paint);
        }
    }
}