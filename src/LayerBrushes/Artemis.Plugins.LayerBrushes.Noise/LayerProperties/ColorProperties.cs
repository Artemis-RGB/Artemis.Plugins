using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class ColorProperties : LayerPropertyGroup
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

        protected override void DisableProperties()
        {
        }

        protected override void EnableProperties()
        {
            Segments.IsVisibleWhen(SegmentColors, s => s.CurrentValue);
            GradientColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Gradient);
            MainColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Simple);
            SecondaryColor.IsVisibleWhen(ColorType, c => c.BaseValue == ColorMappingType.Simple);
        }

        protected override void PopulateDefaults()
        {
            ColorType.DefaultValue = ColorMappingType.Gradient;
            MainColor.DefaultValue = new SKColor(255, 0, 0);
            SecondaryColor.DefaultValue = new SKColor(0, 0, 255);
            GradientColor.DefaultValue = ColorGradient.GetUnicornBarf();
            SegmentColors.DefaultValue = false;
            Segments.DefaultValue = 2;
        }
    }
}
