using System;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.Plugins.LayerBrushes.Particle.Particle.Shapes;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle
{
    internal class Particle
    {
        private static readonly Random Rand = new();
        private SKPoint _acceleration = SKPoint.Empty;
        private float _height;
        private SKPoint _location;
        private float _width;

        public Particle(ParticleConfiguration configuration)
        {
            Configuration = configuration;
            Width = configuration.GetWidth();
            Height = configuration.GetHeight();

            RotationVelocityX = configuration.GetRotationVelocityX();
            RotationVelocityY = configuration.GetRotationVelocityY();
            RotationVelocityZ = configuration.GetRotationVelocityZ();
            if (RotationVelocityX != 0)
                RotationX = Rand.Next(0, 360);
            if (RotationVelocityY != 0)
                RotationY = Rand.Next(0, 360);
            if (RotationVelocityZ != 0)
                RotationZ = Rand.Next(0, 360);

            Shape = configuration.ParticleType switch
            {
                ParticleType.Rectangle => new ParticleRectShape(),
                ParticleType.Ellipse => new ParticleCircleShape(),
                ParticleType.Path => new ParticlePathShape(SKPath.ParseSvgPathData(Configuration.Path)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public ParticleConfiguration Configuration { get; }

        public SKPoint Location
        {
            get => _location;
            set
            {
                _location = value;
                Bounds = new SKRect(_location.X, _location.Y, _location.X + _width, _location.Y + _height);
            }
        }

        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                Bounds = new SKRect(_location.X, _location.Y, _location.X + _width, _location.Y + _height);
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                Bounds = new SKRect(_location.X, _location.Y, _location.X + _width, _location.Y + _height);
            }
        }

        public float Mass { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public SKColorF Color { get; set; }
        public ParticleShape Shape { get; set; }
        public SKPoint Velocity { get; set; }
        public SKPoint MaximumVelocity { get; set; }
        public float RotationVelocityX { get; set; }
        public float RotationVelocityY { get; set; }
        public float RotationVelocityZ { get; set; }
        public bool FadeOut { get; set; }
        public double Lifetime { get; set; }
        public SKRect Bounds { get; private set; }
        public bool IsComplete { get; private set; }

        public void Draw(SKCanvas canvas, TimeSpan deltaTime, SKPaint paint)
        {
            if (IsComplete || Shape == null)
                return;

            canvas.Save();
            SKMatrix matrix = SKMatrix.CreateTranslation(Location.X, Location.Y);

            // Use 3D matrix for 3D rotations and perspective
            SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
            if (RotationX != 0)
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, RotationX));
            if (RotationY != 0)
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, RotationY));
            if (RotationZ != 0)
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, RotationZ));

            matrix = matrix.PreConcat(matrix44.Matrix);
            canvas.SetMatrix(canvas.TotalMatrix.PreConcat(matrix));

            paint.ColorF = Color;
            Shape.Draw(canvas, paint, Width, Height);
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

            RotationX = (RotationX + RotationVelocityX * secs) % 360;
            RotationY = (RotationY + RotationVelocityY * secs) % 360;
            RotationZ = (RotationZ + RotationVelocityZ * secs) % 360;
        }
    }
}