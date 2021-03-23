using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes
{
    public class SKConfettiSquareShape : SKConfettiShape
    {
        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float size)
        {
            float offset = -size / 2f;
            SKRect rect = SKRect.Create(offset, offset, size, size);
            canvas.DrawRect(rect, paint);
        }
    }
}