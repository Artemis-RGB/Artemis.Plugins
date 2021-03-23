using System;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes
{
    public class SKConfettiPathShape : SKConfettiShape
    {
        private SKPath _path;
        private SKSize _baseSize;

        public SKConfettiPathShape()
        {
        }

        public SKConfettiPathShape(SKPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public SKPath? Path
        {
            get => _path;
            set
            {
                _path = value;
                _baseSize = value?.TightBounds.Size ?? SKSize.Empty;
            }
        }

        protected override void OnDraw(SKCanvas canvas, SKPaint paint, float size)
        {
            if (_baseSize.Width <= 0 || _baseSize.Height <= 0 || Path == null)
                return;

            canvas.Save();
            canvas.Scale(size / _baseSize.Width, size / _baseSize.Height);

            canvas.DrawPath(Path, paint);

            canvas.Restore();
        }
    }
}