using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes
{
    public abstract class SKConfettiShape
    {
        public void Draw(SKCanvas canvas, SKPaint paint, float size)
        {
            OnDraw(canvas, paint, size);
        }

        protected abstract void OnDraw(SKCanvas canvas, SKPaint paint, float size);
    }
}