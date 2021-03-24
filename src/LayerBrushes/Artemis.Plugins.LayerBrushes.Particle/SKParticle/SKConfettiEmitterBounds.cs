using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle
{
    public readonly struct SKConfettiEmitterBounds
    {
        public SKConfettiEmitterBounds(SKConfettiEmitterSide side)
            : this(SKRect.Empty, side)
        {
        }

        public SKConfettiEmitterBounds(float x, float y)
            : this(SKRect.Create(x, y, 0, 0), SKConfettiEmitterSide.Bounds)
        {
        }

        public SKConfettiEmitterBounds(SKPoint point)
            : this(SKRect.Create(point, SKSize.Empty), SKConfettiEmitterSide.Bounds)
        {
        }

        public SKConfettiEmitterBounds(float x, float y, float width, float height)
            : this(SKRect.Create(x, y, width, height), SKConfettiEmitterSide.Bounds)
        {
        }

        public SKConfettiEmitterBounds(SKRect rect)
            : this(rect, SKConfettiEmitterSide.Bounds)
        {
        }

        private SKConfettiEmitterBounds(SKRect rect, SKConfettiEmitterSide side)
        {
            Rect = rect;
            Side = side;
        }

        public SKRect Rect { get; }

        public SKConfettiEmitterSide Side { get; }


        public static implicit operator SKConfettiEmitterBounds(SKConfettiEmitterSide side)
        {
            return new(side);
        }

        public static implicit operator SKConfettiEmitterBounds(SKPoint point)
        {
            return new(point);
        }

        public static implicit operator SKConfettiEmitterBounds(SKRect rect)
        {
            return new(rect);
        }

        public static SKConfettiEmitterBounds Top => new(SKConfettiEmitterSide.Top);

        public static SKConfettiEmitterBounds Left => new(SKConfettiEmitterSide.Left);

        public static SKConfettiEmitterBounds Right => new(SKConfettiEmitterSide.Right);

        public static SKConfettiEmitterBounds Bottom => new(SKConfettiEmitterSide.Bottom);

        public static SKConfettiEmitterBounds Center => new(SKConfettiEmitterSide.Center);

        public static SKConfettiEmitterBounds Bounds(float x, float y, float width, float height)
        {
            return new(x, y, width, height);
        }

        public static SKConfettiEmitterBounds Bounds(SKRect rect)
        {
            return new(rect);
        }

        public static SKConfettiEmitterBounds Point(float x, float y)
        {
            return new(x, y);
        }

        public static SKConfettiEmitterBounds Point(SKPoint point)
        {
            return new(point);
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