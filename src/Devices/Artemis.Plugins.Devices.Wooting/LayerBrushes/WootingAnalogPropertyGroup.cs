using Artemis.Core;

namespace Artemis.Plugins.Devices.Wooting.LayerBrushes
{
    internal class WootingAnalogPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription]
        public ColorGradientLayerProperty Color { get; set; }

        protected override void DisableProperties()
        {
        }

        protected override void EnableProperties()
        {
        }

        protected override void PopulateDefaults()
        {
            Color.DefaultValue = ColorGradient.GetUnicornBarf();
        }
    }
}