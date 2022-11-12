using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Particle.LayerProperties;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle
{
    public class ParticleSystem
    {
        private readonly LinkedList<Particle> _particles = new();
        private readonly Random _random = new();
        private SKRect _actualEmitterBounds;
        private ParticleEmitter _emitter;
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                UpdateIsComplete();
            }
        }

        public ParticleEmitter Emitter
        {
            get => _emitter;
            set
            {
                ParticleEmitter oldValue = _emitter;
                _emitter = value;
                OnEmitterChanged(oldValue, _emitter);
            }
        }

        public ParticleEmitterBounds EmitterBounds { get; set; } = new(SKConfettiEmitterSide.Center);
        public SKRect Bounds { get; set; }
        public ColorGradient Colors { get; set; } = new();
        public List<float> Masses { get; set; } = CreateDefaultMasses();
        public List<ParticleConfiguration> Configurations { get; set; } = new();
        public float StartAngle { get; set; }
        public float EndAngle { get; set; } = 360f;
        public float MinimumInitialVelocity { get; set; } = 100f;
        public float MaximumInitialVelocity { get; set; } = 200f;
        public float MaximumVelocity { get; set; } = 0f;
        public float Lifetime { get; set; } = 2f;
        public bool FadeOut { get; set; } = true;
        public SKPoint Gravity { get; set; } = new(0, 9.81f);
        public bool IsComplete { get; set; }
        public ParticleColorMode ParticleColorMode { get; set; }

        public void Update(TimeSpan deltaTime)
        {
            if (IsRunning)
                Emitter?.Update(deltaTime);

            SKPoint g = Gravity;
            bool removed = false;

            LinkedListNode<Particle> particle = _particles.First;
            while (particle != null)
            {
                particle.Value.ApplyForce(g, deltaTime);
                particle.Value.ApplyLifetimeColor(Colors, ParticleColorMode == ParticleColorMode.Lifetime);

                LinkedListNode<Particle> next = particle.Next;
                if (particle.Value.IsComplete)
                {
                    _particles.Remove(particle);
                    removed = true;
                }

                particle = next;
            }

            if (removed)
                UpdateIsComplete();
        }

        public void Draw(SKCanvas canvas, SKPaint paint)
        {
            foreach (Particle particle in _particles) 
                particle.Draw(canvas, paint);
        }

        public void UpdateEmitterBounds(float width, float height)
        {
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
                Masses == null || Masses.Count == 0 ||
                Configurations == null || Configurations.Count == 0)
                return;

            SKColor[] colors = Colors.Colors;
            for (int i = 0; i < count; i++)
            {
                SKColor c = colors[_random.Next(Colors.Count)];
                float mass = Masses[_random.Next(Masses.Count)];
                ParticleConfiguration conf = Configurations[_random.Next(Configurations.Count)];
                Particle particle = new(conf)
                {
                    Location = GetNewLocation(),
                    Velocity = GetNewVelocity(),
                    Color = c,
                    Mass = mass,
                    MaximumVelocity = new SKPoint(MaximumVelocity, MaximumVelocity),
                    FadeOut = FadeOut,
                    Lifetime = Lifetime,
                    TotalLifetime = Lifetime
                };

                _particles.AddLast(particle);
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
        }

        private void OnEmitterChanged(ParticleEmitter oldValue, ParticleEmitter newValue)
        {
            if (oldValue != null)
                oldValue.ParticlesCreated -= OnCreateParticle;
            if (newValue != null)
                newValue.ParticlesCreated += OnCreateParticle;
            UpdateIsComplete();
        }

        private void UpdateIsComplete()
        {
            IsComplete = _particles.Count == 0 && Emitter?.IsComplete != false && IsRunning;
        }

        private static List<float> CreateDefaultMasses()
        {
            return new()
            {
                2,
                3
            };
        }
    }
}