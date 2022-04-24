using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class ColorAdjustmentEffectProperties : LayerEffectPropertyGroup
    {
        [PropertyDescription]
        public EnumLayerProperty<ColorAdjustmentType> Type { get; set; }

        [PropertyDescription]
        public FloatLayerProperty Amount { get; set; }

        protected override void PopulateDefaults()
        {
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum ColorAdjustmentType
    {
        Hue,
        Brightness,
        Contrast,
        Saturation
    }
}