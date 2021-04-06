using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle
{
    public readonly struct ParticleEmitterBounds
    {
        public ParticleEmitterBounds(SKConfettiEmitterSide side)
            : this(SKRect.Empty, side)
        {
        }

        public ParticleEmitterBounds(SKPoint point)
            : this(SKRect.Create(point, SKSize.Empty), SKConfettiEmitterSide.Bounds)
        {
        }

        public ParticleEmitterBounds(float x, float y, float width, float height)
            : this(SKRect.Create(x, y, width, height), SKConfettiEmitterSide.Bounds)
        {
        }

        public ParticleEmitterBounds(SKRect rect)
            : this(rect, SKConfettiEmitterSide.Bounds)
        {
        }

        private ParticleEmitterBounds(SKRect rect, SKConfettiEmitterSide side)
        {
            Rect = rect;
            Side = side;
        }

        public SKRect Rect { get; }

        public SKConfettiEmitterSide Side { get; }


        public static implicit operator ParticleEmitterBounds(SKConfettiEmitterSide side)
        {
            return new(side);
        }

        public static implicit operator ParticleEmitterBounds(SKPoint point)
        {
            return new(point);
        }

        public static implicit operator ParticleEmitterBounds(SKRect rect)
        {
            return new(rect);
        }
    }

    public enum SKConfettiEmitterSide
    {
        Top,
        Right,
        Bottom,
        Left,
        Center,
        Bounds
    }
}