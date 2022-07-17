using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class ColorProperties : LayerPropertyGroup
    {
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
        }

        protected override void PopulateDefaults()
        {
            GradientColor.DefaultValue = ColorGradient.GetUnicornBarf();
            SegmentColors.DefaultValue = false;
            Segments.DefaultValue = 2;
        }
    }
}
