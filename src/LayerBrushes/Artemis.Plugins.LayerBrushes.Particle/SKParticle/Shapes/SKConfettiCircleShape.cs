using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes
{
    public class SKConfettiCircleShape : SKConfettiShape
    {
        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float size)
        {
            canvas.DrawCircle(0, 0, size / 2f, paint);
        }
    }
}