using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class RadialGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }

        [PropertyDescription(Name = "Center offset", Description = "Change the position of the gradient by offsetting it from the center of the layer", InputAffix = "%")]
        public SKPointLayerProperty CenterOffset { get; set; }

        [PropertyDescription(Name = "Resize mode", Description = "How to make the gradient adjust to scale changes")]
        public EnumLayerProperty<RadialGradientResizeMode> ResizeMode { get; set; }

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            ResizeMode.DefaultValue = RadialGradientResizeMode.Fit;
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum RadialGradientResizeMode
    {
        Fit,
        Fill,
        Stretch
    }
}