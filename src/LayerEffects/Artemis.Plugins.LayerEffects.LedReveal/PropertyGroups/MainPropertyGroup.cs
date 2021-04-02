using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.LedReveal.PropertyGroups
{
    public class MainPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription(InputAffix = "%", MinInputValue = 0, MaxInputValue = 100)]
        public FloatLayerProperty Percentage { get; set; }

        [PropertyDescription(Description = "The rounding function to apply to the percentage")]
        public EnumLayerProperty<RoundingFunction> RoundingFunction { get; set; }

        [PropertyDescription(Name = "Limit visible LEDs", Description = "Enables the max visible LEDs option")]
        public BoolLayerProperty LimitVisibleLeds { get; set; }

        [PropertyDescription(Name = "Max visible LEDs", Description = "The last X amount of LEDs to show", MinInputValue = 1)]
        public IntLayerProperty MaxVisibleLeds { get; set; }

        protected override void PopulateDefaults()
        {
            LimitVisibleLeds.DefaultValue = false;
            MaxVisibleLeds.DefaultValue = 1;
            Percentage.DefaultValue = 100f;
        }

        protected override void EnableProperties()
        {
            MaxVisibleLeds.IsVisibleWhen(LimitVisibleLeds, s => s);
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum RoundingFunction
    {
        Round,
        Floor,
        Ceiling
    }
}