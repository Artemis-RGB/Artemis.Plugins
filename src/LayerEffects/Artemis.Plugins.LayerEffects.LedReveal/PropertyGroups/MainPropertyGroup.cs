using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.LedReveal.PropertyGroups
{
    public class MainPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription(InputAffix = "%", MinInputValue = 0, MaxInputValue = 100)]
        public FloatLayerProperty Percentage { get; set; }

        [PropertyDescription]
        public BoolLayerProperty ShowAllRevealedLeds { get; set; }

        [PropertyDescription(MinInputValue = 1)]
        public IntLayerProperty MaxShownLeds { get; set; }

        protected override void PopulateDefaults()
        {
            ShowAllRevealedLeds.DefaultValue = true;
            MaxShownLeds.DefaultValue = 1;
            Percentage.DefaultValue = 100f;
        }

        protected override void EnableProperties()
        {
            MaxShownLeds.IsVisibleWhen(ShowAllRevealedLeds, s => !s);
        }

        protected override void DisableProperties()
        {
        }
    }
}