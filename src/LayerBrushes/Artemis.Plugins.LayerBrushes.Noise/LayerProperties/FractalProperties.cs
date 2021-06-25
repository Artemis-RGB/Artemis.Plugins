using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class FractalProperties : LayerPropertyGroup
    {
        [PropertyDescription]
        public EnumLayerProperty<PropertiesFractalType> FractalType { get; set; }

        [PropertyDescription]
        public IntLayerProperty Octaves { get; set; }

        [PropertyDescription(Description = "The octave lacunarity to use for all fractal noise types")]
        public FloatLayerProperty Lacunarity { get; set; }

        [PropertyDescription(Description = "The octave gain for all fractal noise types")]
        public FloatLayerProperty Gain { get; set; }

        [PropertyDescription(Description = "The octave weighting for all none DomainWarp fratal types", MinInputValue = 0f, MaxInputValue = 1f)]
        public FloatLayerProperty WeightedStrength { get; set; }

        [PropertyDescription]
        public FloatLayerProperty PingPongStrength { get; set; }

        protected override void DisableProperties()
        {
        }

        protected override void EnableProperties()
        {
            Octaves.IsVisibleWhen(FractalType, f => f.CurrentValue != PropertiesFractalType.None);
            Lacunarity.IsVisibleWhen(FractalType, f => f.CurrentValue != PropertiesFractalType.None);
            Gain.IsVisibleWhen(FractalType, f => f.CurrentValue != PropertiesFractalType.None);
            WeightedStrength.IsVisibleWhen(FractalType, f => f.CurrentValue != PropertiesFractalType.None);
            PingPongStrength.IsVisibleWhen(FractalType, f => f.CurrentValue == PropertiesFractalType.PingPong);
        }

        protected override void PopulateDefaults()
        {
            Octaves.DefaultValue = 5;
            Lacunarity.DefaultValue = 2f;
            Gain.DefaultValue = 0.5f;
            WeightedStrength.DefaultValue = 0;
            PingPongStrength.DefaultValue = 2f;
        }

        public enum PropertiesFractalType
        {
            None,
            FBm,
            Ridged,
            PingPong
        };
    }
}