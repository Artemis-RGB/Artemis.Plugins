using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle.Shapes
{
    public abstract class ParticleShape
    {
        public void Draw(SKCanvas canvas, SKPaint paint, float width, float height)
        {
            OnDraw(canvas, paint, width, height);
        }

        protected abstract void OnDraw(SKCanvas canvas, SKPaint paint, float width, float height);
    }
}