using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.LayerProperties;
using Artemis.Plugins.LayerBrushes.Particle.LayerProperties.Presets;
using Artemis.Plugins.LayerBrushes.Particle.Particle;
using Artemis.Plugins.LayerBrushes.Particle.ViewModels;
using Artemis.UI.Shared.LayerBrushes;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle
{
    // This is the layer brush, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class ParticleLayerBrush : LayerBrush<MainPropertyGroup>
    {
        private SKPaint _paint;
        private ParticleSystem _particleSystem;

        public override List<ILayerBrushPreset> Presets => new()
        {
            new FireplacePreset(this),
            new SnowPreset(this),
            new StarsPreset(this)
        };

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
            _particleSystem.StartAngle = Properties.Emitter.Angle - Properties.Emitter.Spread / 2f;
            _particleSystem.EndAngle = Properties.Emitter.Angle + Properties.Emitter.Spread / 2f;

            _particleSystem.MinimumInitialVelocity = Properties.Particles.InitialVelocity.CurrentValue.Start;
            _particleSystem.MaximumInitialVelocity = Properties.Particles.InitialVelocity.CurrentValue.End;
            _particleSystem.MaximumVelocity = Properties.Particles.MaximumVelocity;
            _particleSystem.Lifetime = Properties.Particles.Lifetime;
            _particleSystem.FadeOut = Properties.Particles.FadeOut;
            _particleSystem.Gravity = Properties.Gravity;
            _particleSystem.Colors = Properties.Particles.Colors.CurrentValue.GetColorsArray().ToList();
            _particleSystem.Configurations = Properties.ParticleConfigurations.CurrentValue;
            _particleSystem.Emitter.ParticleRate = Properties.Emitter.EmitParticles ? Properties.Emitter.ParticleRate : 0;

            if (deltaTime > 0)
                _particleSystem.Update(TimeSpan.FromSeconds(deltaTime));
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
            _particleSystem.Bounds = SKRect.Create(-10, -10, bounds.Width + 20, bounds.Height + 20);

            canvas.SaveLayer(paint);
            canvas.Translate(bounds.Left, bounds.Top);
            _particleSystem.Draw(canvas, _paint);
            _paint.Reset();
            canvas.Restore();
        }
    }
}