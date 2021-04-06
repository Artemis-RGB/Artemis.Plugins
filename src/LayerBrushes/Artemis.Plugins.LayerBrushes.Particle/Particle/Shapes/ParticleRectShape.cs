using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle.Shapes
{
    public class ParticleRectShape : ParticleShape
    {
        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float width, float height)
        {
            SKRect rect = SKRect.Create(width / 2f * -1, width / 2f * -1, width, width);
            canvas.DrawRect(rect, paint);
        }
    }
}