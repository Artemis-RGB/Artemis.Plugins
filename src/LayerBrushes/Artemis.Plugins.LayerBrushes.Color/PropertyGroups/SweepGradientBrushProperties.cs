using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class SweepGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }
        
        [PropertyDescription(Description = "The position of the gradient", InputStepSize = 0.001f)]
        public SKPointLayerProperty Position { get; set; }

        [PropertyDescription(Description = "Change the angle at which the sweep starts with the first color", InputAffix = "°")]
        public FloatLayerProperty StartAngle { get; set; }

        [PropertyDescription(Description = "Change the angle at which the sweep ends with the last color", InputAffix = "°")]
        public FloatLayerProperty EndAngle { get; set; }

        [PropertyDescription(Description = "Change speed at which the gradient rotates in degrees per second. \r\n" +
                                           "A positive value will result in a clockwise rotation and a negative value in a counter-clockwise rotation", InputAffix = "°/s")]
        public FloatLayerProperty RotateSpeed { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            Position.DefaultValue = new SKPoint(0.5f, 0.5f);
            EndAngle.DefaultValue = 360;
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