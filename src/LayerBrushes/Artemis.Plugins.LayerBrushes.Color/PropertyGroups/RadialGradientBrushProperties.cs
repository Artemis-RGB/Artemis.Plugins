using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class RadialGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }
        
        [PropertyDescription(Description = "The position of the gradient", InputStepSize = 0.001f)]
        public SKPointLayerProperty Position { get; set; }

        [PropertyDescription(Name = "Resize mode", Description = "How to make the gradient adjust to scale changes")]
        public EnumLayerProperty<RadialGradientResizeMode> ResizeMode { get; set; }
        
        [PropertyDescription(Name = "Zoom speed", Description = "How fast to zoom the gradient, if set to 0 is static, a negative value zooms out", InputAffix = "p/s")]
        public FloatLayerProperty ZoomSpeed { get; set; }

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            Position.DefaultValue = new SKPoint(0.5f, 0.5f);
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