using Artemis.Core;

namespace Artemis.Plugins.LayerEffect.Strobe.PropertyGroups
{
    public class MainPropertyGroup : LayerEffectPropertyGroup
    {
        [PropertyDescription(Description = "How often to strobe per second", InputAffix = "p/s", MinInputValue = 0, MaxInputValue = 10)]
        public FloatLayerProperty Speed { get; set; }

        [PropertyDescription(Description = "Whether or not to invert the strobing, useful when creating two opposites")]
        public BoolLayerProperty Inverted { get; set; }

        public EnumLayerProperty<StrobeTransitionMode> BrightenTransitionMode { get; set; }
        public EnumLayerProperty<StrobeTransitionMode> DimTransitionMode { get; set; }

        protected override void PopulateDefaults()
        {
            Speed.DefaultValue = 1f;
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum StrobeTransitionMode
    {
        None,
        Linear,
        Eased
    }
}