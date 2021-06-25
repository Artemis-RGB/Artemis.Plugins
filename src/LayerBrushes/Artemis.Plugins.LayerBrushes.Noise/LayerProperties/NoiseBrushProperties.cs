using Artemis.Core;
using SkiaSharp;
using static FastNoiseLite;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription]
        public EnumLayerProperty<NoiseType> NoiseType { get; set; }

        [PropertyDescription(Description = "The scale of the noise", MinInputValue = 0f, InputAffix = "%")]
        public SKSizeLayerProperty Scale { get; set; }

        [PropertyDescription(Description = "The speed at which the noise moves vertically and horizontally", MinInputValue = -64f, MaxInputValue = 64f)]
        public SKPointLayerProperty ScrollSpeed { get; set; }

        [PropertyDescription(Description = "The speed at which the noise moves", MinInputValue = 0f, MaxInputValue = 64f)]
        public FloatLayerProperty AnimationSpeed { get; set; }

        [PropertyDescription]
        public ColorProperties Colors { get; set; }

        [PropertyDescription]
        public FractalProperties Fractal { get; set; }

        [PropertyDescription]
        public CellularProperties Cellular { get; set; }

        protected override void PopulateDefaults()
        {
            Scale.DefaultValue = new SKSize(100, 100);
            AnimationSpeed.DefaultValue = 25f;
        }

        protected override void EnableProperties()
        {
            NoiseType.CurrentValueSet += NoiseTypeCurrentValueSet;
            Cellular.IsHidden = NoiseType.CurrentValue != FastNoiseLite.NoiseType.Cellular;
        }

        protected override void DisableProperties()
        {
            NoiseType.CurrentValueSet -= NoiseTypeCurrentValueSet;
        }

        private void NoiseTypeCurrentValueSet(object sender, LayerPropertyEventArgs e)
        {
            Cellular.IsHidden = NoiseType.CurrentValue != FastNoiseLite.NoiseType.Cellular;
        }
    }
}