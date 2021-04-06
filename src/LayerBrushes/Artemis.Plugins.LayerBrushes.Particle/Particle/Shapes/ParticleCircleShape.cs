using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle.Shapes
{
    public class ParticleCircleShape : ParticleShape
    {
        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float width, float height)
        {
            canvas.DrawOval(0, 0, width / 2f, height / 2, paint);
        }
    }
}