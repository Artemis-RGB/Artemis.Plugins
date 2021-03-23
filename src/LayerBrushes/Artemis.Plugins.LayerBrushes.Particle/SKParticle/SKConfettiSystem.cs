using System;
using System.Collections.Generic;
using Artemis.Plugins.LayerBrushes.Particle.SKParticle.Shapes;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle
{
    public class SKConfettiSystem
    {
        private readonly List<SKConfettiParticle> _particles = new();
        private readonly Random _random = new();
        private SKRect _actualEmitterBounds;
        private SKConfettiEmitter _emitter;
        private bool _isRunning;
        private SKRect _lastViewBounds;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                UpdateIsComplete();
            }
        }

        public SKConfettiEmitter Emitter
        {
            get => _emitter;
            set
            {
                SKConfettiEmitter oldValue = _emitter;
                _emitter = value;
                OnEmitterChanged(oldValue, _emitter);
            }
        }

        public SKConfettiEmitterBounds EmitterBounds { get; set; } = SKConfettiEmitterBounds.Top;
        public List<SKColor> Colors { get; set; } = CreateDefaultColors();
        public List<SKConfettiPhysics> Physics { get; set; } = CreateDefaultPhysics();
        public List<SKConfettiShape> Shapes { get; set; } = CreateDefaultShapes();
        public float StartAngle { get; set; }
        public float EndAngle { get; set; } = 360f;
        public float MinimumInitialVelocity { get; set; } = 100f;
        public float MaximumInitialVelocity { get; set; } = 200f;
        public float MinimumRotationVelocity { get; set; } = 10f;
        public float MaximumRotationVelocity { get; set; } = 75f;
        public float MaximumVelocity { get; set; } = 0f;
        public float Lifetime { get; set; } = 2f;
        public bool FadeOut { get; set; } = true;
        public SKPoint Gravity { get; set; } = new(0, 9.81f);
        public bool IsComplete { get; set; }
        internal int ParticleCount => _particles.Count;

        public void Draw(SKCanvas canvas, TimeSpan deltaTime, SKPaint paint)
        {
            if (IsRunning)
                Emitter?.Update(deltaTime);

            SKPoint g = Gravity;

            bool removed = false;
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                SKConfettiParticle particle = _particles[i];

                particle.ApplyForce(g, deltaTime);

                if (!particle.IsComplete && _lastViewBounds.IntersectsWith(particle.Bounds))
                {
                    particle.Draw(canvas, deltaTime, paint);
                }
                else
                {
                    _particles.RemoveAt(i);
                    removed = true;
                }
            }

            if (removed)
                UpdateIsComplete();
        }

        public void UpdateEmitterBounds(float width, float height)
        {
            _lastViewBounds = new SKRect(0, 0, width, height);

            _actualEmitterBounds = EmitterBounds.Side switch
            {
                SKConfettiEmitterSide.Top => SKRect.Create(0, -10, width, 0),
                SKConfettiEmitterSide.Left => SKRect.Create(-10, 0, 0, height),
                SKConfettiEmitterSide.Right => SKRect.Create(width + 10, 0, 0, height),
                SKConfettiEmitterSide.Bottom => SKRect.Create(0, height + 10, width, 0),
                SKConfettiEmitterSide.Center => SKRect.Create(width / 2, height / 2, 0, 0),
                _ => EmitterBounds.Rect
            };
        }

        private void OnCreateParticle(int count)
        {
            if (Colors == null || Colors.Count == 0 ||
                Physics == null || Physics.Count == 0 ||
                Shapes == null || Shapes.Count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                SKColor c = Colors[_random.Next(Colors.Count)];
                SKConfettiPhysics p = Physics[_random.Next(Physics.Count)];
                SKConfettiShape s = Shapes[_random.Next(Shapes.Count)];

                SKConfettiParticle particle = new()
                {
                    Location = GetNewLocation(),
                    Velocity = GetNewVelocity(),
                    RotationVelocity = GetNewRotationVelocity(),

                    Color = c,
                    Size = p.Size,
                    Mass = p.Mass,
                    Shape = s,
                    Rotation = GetNewRotation(),

                    MaximumVelocity = new SKPoint(MaximumVelocity, MaximumVelocity),
                    FadeOut = FadeOut,
                    Lifetime = Lifetime
                };

                _particles.Add(particle);
            }

            UpdateIsComplete();

            SKPoint GetNewLocation()
            {
                SKRect rect = _actualEmitterBounds;
                return new SKPoint(
                    rect.Left + (float) _random.NextDouble() * rect.Width,
                    rect.Top + (float) _random.NextDouble() * rect.Height);
            }

            SKPoint GetNewVelocity()
            {
                float velocity = MinimumInitialVelocity + (float) _random.NextDouble() * (MaximumInitialVelocity - MinimumInitialVelocity);
                float deg = StartAngle + (float) _random.NextDouble() * (EndAngle - StartAngle);
                float rad = MathF.PI / 180f * deg;

                float vx = velocity * MathF.Cos(rad);
                float vy = velocity * MathF.Sin(rad);

                return new SKPoint(vx, vy);
            }

            float GetNewRotationVelocity()
            {
                if (MaximumRotationVelocity < MinimumRotationVelocity)
                    return 0;

                return MinimumRotationVelocity + (float) _random.NextDouble() * (MaximumRotationVelocity - MinimumRotationVelocity);
            }

            float GetNewRotation()
            {
                return (float) _random.NextDouble() * 360f;
            }
        }

        private void OnEmitterChanged(SKConfettiEmitter oldValue, SKConfettiEmitter newValue)
        {
            if (oldValue != null)
                oldValue.ParticlesCreated -= OnCreateParticle;
            if (newValue != null)
                newValue.ParticlesCreated += OnCreateParticle;
            UpdateIsComplete();
        }

        private bool UpdateIsComplete()
        {
            return IsComplete =
                _particles.Count == 0 &&
                Emitter?.IsComplete != false &&
                IsRunning;
        }

        private static List<SKColor> CreateDefaultColors()
        {
            return new()
            {
                new SKColor(0xfffce18a),
                new SKColor(0xffff726d),
                new SKColor(0xffb48def),
                new SKColor(0xfff4306d),
                new SKColor(0xff3aaab8),
                new SKColor(0xff38ba9e),
                new SKColor(0xffbb3d72),
                new SKColor(0xff006ded)
            };
        }

        private static List<SKConfettiPhysics> CreateDefaultPhysics()
        {
            return new()
            {
                new SKConfettiPhysics(60, 2),
                new SKConfettiPhysics(80, 3)
            };
        }

        private static List<SKConfettiShape> CreateDefaultShapes()
        {
            return new()
            {
                new SKConfettiSquareShape(),
                new SKConfettiCircleShape(),
                new SKConfettiRectShape(0.5),
                new SKConfettiOvalShape(0.5),
                new SKConfettiRectShape(0.1)
            };
        }
    }
}