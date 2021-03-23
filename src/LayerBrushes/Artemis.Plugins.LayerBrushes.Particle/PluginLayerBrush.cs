using System;
using System.Linq;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.PropertyGroups;
using Artemis.Plugins.LayerBrushes.Particle.SKParticle;
using SkiaSharp;


namespace Artemis.Plugins.LayerBrushes.Particle
{
    // This is the layer brush, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class PluginLayerBrush : LayerBrush<MainPropertyGroup>
    {
        private SKConfettiSystem _particleSystem;
        private double _lastDelta;

        public override void EnableLayerBrush()
        {
            _particleSystem = new SKConfettiSystem {Emitter = new SKConfettiEmitter(200, -1), IsRunning = true};
        }

        public override void DisableLayerBrush()
        {
        }

        public override void Update(double deltaTime)
        {
            _lastDelta = deltaTime;
            _particleSystem.StartAngle = Properties.Angle - Properties.Spread / 2f;
            _particleSystem.EndAngle = Properties.Angle + Properties.Spread / 2f;

            _particleSystem.MinimumInitialVelocity = Properties.InitialVelocity.CurrentValue.Start;
            _particleSystem.MaximumInitialVelocity = Properties.InitialVelocity.CurrentValue.End;
            _particleSystem.MinimumRotationVelocity = Properties.RotationVelocity.CurrentValue.Start;
            _particleSystem.MaximumRotationVelocity = Properties.RotationVelocity.CurrentValue.End;
            _particleSystem.MaximumVelocity = Properties.MaximumVelocity.CurrentValue;
            _particleSystem.Lifetime = Properties.Lifetime.CurrentValue;
            _particleSystem.FadeOut = Properties.FadeOut.CurrentValue;
            _particleSystem.Gravity = Properties.Gravity.CurrentValue;
            _particleSystem.EmitterBounds = new SKConfettiEmitterBounds(Properties.EmitterSide.CurrentValue);

            _particleSystem.Colors = Properties.Colors.CurrentValue.GetColorsArray().ToList();
            _particleSystem.Emitter.ParticleRate = Properties.ParticleRate.CurrentValue;
        }

        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            _particleSystem.UpdateEmitterBounds(bounds.Width, bounds.Height);
            _particleSystem.Draw(canvas, TimeSpan.FromSeconds(Math.Max(0, _lastDelta)), paint);
        }
    }
}