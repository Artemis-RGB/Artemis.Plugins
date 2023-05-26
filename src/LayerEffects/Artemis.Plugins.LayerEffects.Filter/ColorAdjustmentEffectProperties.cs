using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class ColorAdjustmentEffectProperties : LayerEffectPropertyGroup
    {
        [PropertyDescription]
        public EnumLayerProperty<ColorAdjustmentType> Type { get; set; }

        [PropertyDescription(Name = "Hue rotation", Description = "The rotation of the hue in degrees", InputAffix = "°")]
        public FloatLayerProperty HueRotation { get; set; }
        
        [PropertyDescription(Description = "The strength of the effect", MinInputValue = -100, MaxInputValue = 100, InputAffix = "%")]
        public FloatLayerProperty Strength { get; set; }

        protected override void PopulateDefaults()
        {
        }

        protected override void EnableProperties()
        {
            HueRotation.IsVisibleWhen(Type, t => t.CurrentValue == ColorAdjustmentType.Hue);
            Strength.IsVisibleWhen(Type, t => t.CurrentValue != ColorAdjustmentType.Hue);
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