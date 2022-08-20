using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class BlurEffectProperties : LayerEffectPropertyGroup
    {
        [PropertyDescription(Description = "The amount of blur to apply")]
        public SKSizeLayerProperty BlurAmount { get; set; }


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
}