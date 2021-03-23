using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class OpacityEffectProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The opacity of the shape", InputAffix = "%", MinInputValue = 0f, MaxInputValue = 100f)]
        public FloatLayerProperty Opacity { get; set; }

        protected override void PopulateDefaults()
        {
            Opacity.DefaultValue = 100f;
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }
}