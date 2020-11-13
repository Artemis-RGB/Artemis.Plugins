using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.ColorRgbNet
{
    public class RgbNetColorBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<RgbNetColorBrush>("RGB.NET Color", "A RGB.NET based color", "Brush");
        }

        public override void Disable()
        {
        }
    }
}