using Artemis.Core;
using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Color
{
    [PluginFeature(AlwaysEnabled = true)]
    public class ColorBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<SolidBrush>("Solid", "Fills the entire layer with a solid color", "Water");
            RegisterLayerBrushDescriptor<LinearGradientBrush>("Linear Gradient", "Fills the entire layer with a linear gradient", "GradientHorizontal");
            RegisterLayerBrushDescriptor<RadialGradientBrush>("Radial Gradient", "Fills the entire layer with a radial gradient", "BlurRadial");
            RegisterLayerBrushDescriptor<SweepGradientBrush>("Sweep Gradient", "Fills the entire layer with a sweep gradient", "Radar");
        }

        public override void Disable()
        {
        }
    }
}