using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.LayerProperties.Presets
{
    public class SnowPreset : ILayerBrushPreset
    {
        private readonly MainPropertyGroup _properties;

        public SnowPreset(ParticleLayerBrush brush)
        {
            _properties = brush.Properties;
        }

        public string Name => "Snow";
        public string Description => "Snow falling down from the top of the layer";
        public string Icon => "SnowflakeVariant";

        public void Apply()
        {
            _properties.ParticleConfigurations.CurrentValue.Clear();
            _properties.ParticleConfigurations.CurrentValue.Add(new ParticleConfiguration
            {
                ParticleType = ParticleType.Ellipse,
                MinHeight = 15,
                MaxHeight = 20,
                MinWidth = 15,
                MaxWidth = 20
            });

            _properties.Emitter.ParticleRate.SetCurrentValue(50, null);
            _properties.Emitter.EmitterPosition.SetCurrentValue(EmitterPosition.Top, null);
            _properties.Emitter.Angle.SetCurrentValue(90, null);
            _properties.Emitter.Spread.SetCurrentValue(45f, null);

            _properties.Particles.Colors.CurrentValue.Clear();
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFEDFDFF), 0f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFC9F9FF), 0.5f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFC9EAFF), 1f));

            _properties.Particles.InitialVelocity.SetCurrentValue(new FloatRange(100f, 120f), null);
            _properties.Particles.MaximumVelocity.SetCurrentValue(0f, null);
            _properties.Particles.Lifetime.SetCurrentValue(2f, null);
            _properties.Particles.FadeOut.SetCurrentValue(false, null);

            _properties.Gravity.SetCurrentValue(new SKPoint(-4f, 4f), null);
        }
    }
}