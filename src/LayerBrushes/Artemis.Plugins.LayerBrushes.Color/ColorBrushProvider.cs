using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class ColorBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<ColorBrush>("Color", "A brush supporting solid colors and multiple types of gradients", "Brush");
        }

        public override void Disable()
        {
        }
    }
}