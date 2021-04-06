using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.LayerProperties.Presets
{
    public class FireplacePreset : ILayerBrushPreset
    {
        private readonly MainPropertyGroup _properties;

        public FireplacePreset(ParticleLayerBrush brush)
        {
            _properties = brush.Properties;
        }

        public string Name => "Fireplace";
        public string Description => "A fireplace originating from the bottom edge of the layer";
        public string Icon => "Fireplace";

        public void Apply()
        {
            _properties.ParticleConfigurations.CurrentValue.Clear();
            _properties.ParticleConfigurations.CurrentValue.Add(new ParticleConfiguration
            {
                ParticleType = ParticleType.Ellipse,
                MinWidth = 30,
                MaxWidth = 35,
                MinHeight = 40,
                MaxHeight = 60
            });

            _properties.Emitter.ParticleRate.SetCurrentValue(200, null);
            _properties.Emitter.EmitterPosition.SetCurrentValue(EmitterPosition.Bottom, null);
            _properties.Emitter.Angle.SetCurrentValue(-75f, null);
            _properties.Emitter.Spread.SetCurrentValue(15f, null);

            _properties.Particles.Colors.CurrentValue.Clear();
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFFF1B00), 0f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFFF7E00), 0.5f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFFF9900), 1f));

            _properties.Particles.InitialVelocity.SetCurrentValue(new FloatRange(100f, 200f), null);
            _properties.Particles.MaximumVelocity.SetCurrentValue(0f, null);
            _properties.Particles.Lifetime.SetCurrentValue(1f, null);
            _properties.Particles.FadeOut.SetCurrentValue(true, null);

            _properties.Gravity.SetCurrentValue(new SKPoint(-4f, 4f), null);
        }
    }
}