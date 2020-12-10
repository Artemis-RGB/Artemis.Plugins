using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class LinearGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }

        [PropertyDescription(Description = "Change the rotation of the gradient without affecting the rotation of the shape", InputAffix = "°")]
        public FloatLayerProperty Rotation { get; set; }

        [PropertyDescription(Description = "The speed at which the gradient moves vertically and horizontally in cm per second.", InputAffix = "cm/s")]
        public SKPointLayerProperty ScrollSpeed { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }

        #endregion
    }
}