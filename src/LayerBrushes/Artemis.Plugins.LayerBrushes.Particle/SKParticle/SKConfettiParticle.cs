using System;
using Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle
{
    internal class SKConfettiParticle
    {
        private SKPoint _acceleration = SKPoint.Empty;
        private SKPoint _location;
        private float _rotationWidth;
        private float _scaleX = 1f;
        private float _size;

        public SKPoint Location
        {
            get => _location;
            set
            {
                _location = value;
                Bounds = new SKRect(_location.X - _size, _location.Y - _size, _size * 2, _size * 2);
            }
        }

        public float Size
        {
            get => _size;
            set
            {
                _size = value;
                Bounds = new SKRect(_location.X - _size, _location.Y - _size, _size * 2, _size * 2);
            }
        }

        public float Mass { get; set; }
        public float Rotation { get; set; }
        public SKColorF Color { get; set; }
        public SKConfettiShape Shape { get; set; }
        public SKPoint Velocity { get; set; }
        public float RotationVelocity { get; set; }
        public SKPoint MaximumVelocity { get; set; }
        public bool FadeOut { get; set; }
        public double Lifetime { get; set; }
        public SKRect Bounds { get; private set; }
        public bool IsComplete { get; private set; }
        
        public void Draw(SKCanvas canvas, TimeSpan deltaTime, SKPaint paint)
        {
            if (IsComplete || Shape == null)
                return;

            canvas.Save();
            canvas.Translate(Location);

            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation);
                canvas.Scale(_scaleX, 1f);
            }

            paint.ColorF = Color;
            Shape.Draw(canvas, paint, Size);
            canvas.Restore();
        }

        public void ApplyForce(SKPoint force, TimeSpan deltaTime)
        {
            if (IsComplete)
                return;

            float secs = (float) deltaTime.TotalSeconds;
            force.X = force.X / Mass * secs;
            force.Y = force.Y / Mass * secs;

            if (force != SKPoint.Empty)
                _acceleration += force;

            Velocity += _acceleration;
            if (MaximumVelocity != SKPoint.Empty)
            {
                float vx = Velocity.X;
                float vy = Velocity.Y;

                vx = vx < 0
                    ? Math.Max(vx, -MaximumVelocity.X)
                    : Math.Min(vx, MaximumVelocity.X);
                vy = vy < 0
                    ? Math.Max(vy, -MaximumVelocity.Y)
                    : Math.Min(vy, MaximumVelocity.Y);

                Velocity = new SKPoint(vx, vy);
            }

            Location = new SKPoint(
                Location.X + Velocity.X * secs,
                Location.Y + Velocity.Y * secs);

            Lifetime -= deltaTime.TotalSeconds;
            if (Lifetime <= 0)
            {
                if (FadeOut)
                {
                    SKColorF c = Color;
                    float alpha = c.Alpha - secs;
                    Color = c.WithAlpha(alpha);
                    IsComplete = alpha <= 0;
                }
                else
                {
                    IsComplete = true;
                }
            }

            if (RotationVelocity != 0)
            {
                float rv = RotationVelocity * secs;

                Rotation += rv;
                if (Rotation >= 360)
                    Rotation = 0f;

                _rotationWidth -= rv;
                if (_rotationWidth < 0)
                    _rotationWidth = Size;

                _scaleX = Math.Abs(_rotationWidth / Size - 0.5f) * 2;
            }
        }
    }
}