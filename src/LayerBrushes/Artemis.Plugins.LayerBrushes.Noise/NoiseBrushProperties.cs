using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Name = "Color mapping type", Description = "The way the noise is converted to colors")]
        public EnumLayerProperty<ColorMappingType> ColorType { get; set; }

        [PropertyDescription(Description = "The main color of the noise")]
        public SKColorLayerProperty MainColor { get; set; }

        [PropertyDescription(Description = "The secondary color of the noise")]
        public SKColorLayerProperty SecondaryColor { get; set; }

        [PropertyDescription(Name = "Noise gradient map", Description = "The gradient the noise will map it's value to")]
        public ColorGradientLayerProperty GradientColor { get; set; }

        [PropertyDescription(Name = "Enable color segmenting", Description = "When enabled you can split the gradient up into segments")]
        public BoolLayerProperty SegmentColors { get; set; }

        [PropertyDescription(Name = "Amount of segments", Description = "The segments in which to split the noise up", MinInputValue = 2, MaxInputValue = 8)]
        public IntLayerProperty Segments { get; set; }

        [PropertyDescription(Description = "The scale of the noise", MinInputValue = 0f, InputAffix = "%")]
        public SKSizeLayerProperty Scale { get; set; }

        [PropertyDescription(Description = "The speed at which the noise moves vertically and horizontally", MinInputValue = -64f, MaxInputValue = 64f)]
        public SKPointLayerProperty ScrollSpeed { get; set; }

        [PropertyDescription(Description = "The speed at which the noise moves", MinInputValue = 0f, MaxInputValue = 64f)]
        public FloatLayerProperty AnimationSpeed { get; set; }

        protected override void PopulateDefaults()
        {
            MainColor.DefaultValue = new SKColor(255, 0, 0);
            SecondaryColor.DefaultValue = new SKColor(0, 0, 255);
            GradientColor.DefaultValue = ColorGradient.GetUnicornBarf();
            Scale.DefaultValue = new SKSize(100, 100);
            SegmentColors.DefaultValue = false;
            Segments.DefaultValue = 2;
            AnimationSpeed.DefaultValue = 25f;
        }

        protected override void EnableProperties()
        {
            Segments.IsVisibleWhen(SegmentColors, s => s.CurrentValue);
            GradientColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Gradient);
            MainColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Simple);
            SecondaryColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Simple);
        }

        protected override void DisableProperties()
        {
        }
    }
}