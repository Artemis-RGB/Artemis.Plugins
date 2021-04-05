using System;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle.Shapes
{
    public class ParticlePathShape : ParticleShape
    {
        private SKSize _baseSize;
        private SKPath _path;

        public ParticlePathShape(SKPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Path.Transform(SKMatrix.CreateTranslation(Path.Bounds.Left * -1 + Path.Bounds.Width / 2f * -1, Path.Bounds.Top * -1 + Path.Bounds.Height / 2f * -1));
        }

        public SKPath Path
        {
            get => _path;
            set
            {
                _path = value;
                _baseSize = value?.TightBounds.Size ?? SKSize.Empty;
            }
        }

        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float width, float height)
        {
            if (_baseSize.Width <= 0 || _baseSize.Height <= 0 || Path == null)
                return;

            canvas.Save();
            canvas.Scale(width / _baseSize.Width, height / _baseSize.Height);
            canvas.DrawPath(Path, paint);
            canvas.Restore();
        }
    }
}