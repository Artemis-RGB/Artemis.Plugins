using System;
using System.Linq;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.Particle;
using Artemis.Plugins.LayerBrushes.Particle.PropertyGroups;
using Artemis.Plugins.LayerBrushes.Particle.ViewModels;
using Artemis.UI.Shared.LayerBrushes;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle
{
    // This is the layer brush, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class ParticleLayerBrush : LayerBrush<MainPropertyGroup>
    {
        private double _lastDelta;
        private ParticleSystem _particleSystem;
        private SKPaint _paint;

        public override void EnableLayerBrush()
        {
            ConfigurationDialog = new LayerBrushConfigurationDialog<ParticlesConfigurationViewModel>();

            _particleSystem = new ParticleSystem
            {
                Emitter = new ParticleEmitter(),
                IsRunning = true
            };
            _paint = new SKPaint();
        }

        public override void DisableLayerBrush()
        {
            _paint?.Dispose();
            _paint = null;
        }

        public override void Update(double deltaTime)
        {
            _lastDelta = deltaTime;
            _particleSystem.StartAngle = Properties.Emitter.Angle - Properties.Emitter.Spread / 2f;
            _particleSystem.EndAngle = Properties.Emitter.Angle + Properties.Emitter.Spread / 2f;

            _particleSystem.MinimumInitialVelocity = Properties.Particles.InitialVelocity.CurrentValue.Start;
            _particleSystem.MaximumInitialVelocity = Properties.Particles.InitialVelocity.CurrentValue.End;
            _particleSystem.MinimumRotationVelocity = Properties.Particles.RotationVelocity.CurrentValue.Start;
            _particleSystem.MaximumRotationVelocity = Properties.Particles.RotationVelocity.CurrentValue.End;
            _particleSystem.MaximumVelocity = Properties.Particles.MaximumVelocity;
            _particleSystem.Lifetime = Properties.Particles.Lifetime;
            _particleSystem.FadeOut = Properties.Particles.FadeOut;
            _particleSystem.Gravity = Properties.Gravity;
            _particleSystem.Colors = Properties.Particles.Colors.CurrentValue.GetColorsArray().ToList();
            _particleSystem.Configurations = Properties.ParticleConfigurations.CurrentValue;
            _particleSystem.Emitter.ParticleRate = Properties.Emitter.ParticleRate;
        }

        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (Properties.Emitter.EmitterPosition != EmitterPosition.Custom)
                _particleSystem.EmitterBounds = new ParticleEmitterBounds((SKConfettiEmitterSide) Properties.Emitter.EmitterPosition.CurrentValue);
            else
                _particleSystem.EmitterBounds = new ParticleEmitterBounds(
                    Properties.Emitter.CustomEmitterPosition.CurrentValue.X / 100f * bounds.Width,
                    Properties.Emitter.CustomEmitterPosition.CurrentValue.Y / 100f * bounds.Height,
                    Properties.Emitter.CustomEmitterSize.CurrentValue.Width / 100f * bounds.Width,
                    Properties.Emitter.CustomEmitterSize.CurrentValue.Height / 100f * bounds.Height
                );

            _particleSystem.UpdateEmitterBounds(bounds.Width, bounds.Height);
            canvas.SaveLayer(paint);
            _particleSystem.Draw(canvas, TimeSpan.FromSeconds(Math.Max(0, _lastDelta)), _paint);
            _paint.Reset();
            canvas.Restore();
        }
    }
}