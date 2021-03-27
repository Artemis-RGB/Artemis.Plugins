using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class LinearGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Description = "Change the length of the visible portion of the gradient without change de size of the shape", InputAffix = "%")]
        public IntLayerProperty WaveSize { get; set; }

        [PropertyDescription(Description = "Change the orientation of the gradient without affecting the orientation of the shape")]
        public EnumLayerProperty<LinearGradientOrientatonMode> Orientation { get; set; }

        [PropertyDescription(Description = "Change how the gradient will be handled when it is painted outside shape bounds")]
        public EnumLayerProperty<LinearGradientRepeatmode> RepeatMode { get; set; }

        [PropertyDescription(Description = "Change de rotation of the gradient without affecting the rotation of the shape", InputAffix = "°")]
        public FloatLayerProperty Rotation { get; set; }

        [PropertyDescription(Description = "The speed at which the gradient moves vertically and horizontally in cm per second.", InputAffix = "cm/s")]
        public SKPointLayerProperty ScrollSpeed { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            WaveSize.DefaultValue = 100;
            Orientation.DefaultValue = LinearGradientOrientatonMode.Horizontal;
            RepeatMode.DefaultValue = LinearGradientRepeatmode.Repeat;
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }

        #endregion
    }

    public enum LinearGradientOrientatonMode
    {
        Horizontal,
        Vertical
    }

    public enum LinearGradientRepeatmode
    {
        Repeat = 1,
        RepeatSeamless = 2,
        Mirror = 3
    }
}