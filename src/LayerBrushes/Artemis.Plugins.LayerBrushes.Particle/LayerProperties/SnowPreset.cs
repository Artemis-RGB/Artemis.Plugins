using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.LayerProperties
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
        public string Icon => "Snowflake";

        public void Apply()
        {
            _properties.Emitter.EmitterPosition.SetCurrentValue(EmitterPosition.Top, null);
            _properties.Emitter.Angle.SetCurrentValue(90f, null);
            _properties.Emitter.ParticleRate.SetCurrentValue(40, null);

            _properties.Particles.Colors.CurrentValue.Clear();
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFEDFDFF), 0f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFC9F9FF), 0.5f));
            _properties.Particles.Colors.CurrentValue.Add(new ColorGradientStop(new SKColor(0xFFC9EAFF), 1f));
            _properties.Particles.FadeOut.SetCurrentValue(false, null);

            _properties.ParticleConfigurations.CurrentValue.Clear();
            _properties.ParticleConfigurations.CurrentValue.Add(new ParticleConfiguration
            {
                ParticleType = ParticleType.Ellipse,
                MinHeight = 15,
                MaxHeight = 20,
                MinWidth = 15,
                MaxWidth = 20
            });
        }
    }
}