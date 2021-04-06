using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.LayerProperties.Presets
{
    public class StarsPreset : ILayerBrushPreset
    {
        private readonly MainPropertyGroup _properties;

        public StarsPreset(ParticleLayerBrush brush)
        {
            _properties = brush.Properties;
        }

        public string Name => "Stars";
        public string Description => "Stars twinkling all across the layer";
        public string Icon => "Creation";

        public void Apply()
        {
            _properties.ParticleConfigurations.CurrentValue.Clear();
            _properties.ParticleConfigurations.CurrentValue.Add(new ParticleConfiguration
            {
                ParticleType = ParticleType.Ellipse,
                MinWidth = 35f,
                MaxWidth = 40f,
                MinHeight = 35f,
                MaxHeight = 40f,
                MaxRotationVelocityX = 90f,
                MaxRotationVelocityY = 90f
            });

            _properties.Emitter.ParticleRate.SetCurrentValue(25, null);
            _properties.Emitter.EmitterPosition.SetCurrentValue(EmitterPosition.Custom, null);
            _properties.Emitter.CustomEmitterPosition.SetCurrentValue(new SKPoint(0f, 0f), null);
            _properties.Emitter.CustomEmitterSize.SetCurrentValue(new SKSize(100f, 100f), null);
            _properties.Emitter.Angle.SetCurrentValue(90, null);
            _properties.Emitter.Spread.SetCurrentValue(45f, null);

            _properties.Particles.Colors.CurrentValue.Clear();
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFC000FF), 0f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFFFFFFF), 0.2f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFF3C1FF), 0.4f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFE676FF), 0.6f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFFF0087), 0.8f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFF0060FF), 1f));

            _properties.Particles.InitialVelocity.SetCurrentValue(new FloatRange(0f, 0f), null);
            _properties.Particles.MaximumVelocity.SetCurrentValue(0f, null);
            _properties.Particles.Lifetime.SetCurrentValue(2f, null);
            _properties.Particles.FadeOut.SetCurrentValue(true, null);

            _properties.Gravity.SetCurrentValue(new SKPoint(0f, 0f), null);
        }
    }
}