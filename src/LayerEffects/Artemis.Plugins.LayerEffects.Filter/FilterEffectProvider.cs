using Artemis.Core;
using Artemis.Core.LayerEffects;

namespace Artemis.Plugins.LayerEffects.Filter
{
    [PluginFeature(AlwaysEnabled = true)]
    public class FilterEffectProvider : LayerEffectProvider
    {
        public override void Enable()
        {
            RegisterLayerEffectDescriptor<BlurEffect>(
                "Blur",
                "A layer effect providing a blur filter effect. \r\nNote: CPU intensive, best to only use on small layers or for a short period of time.",
                "BlurOn"
            );
            RegisterLayerEffectDescriptor<DilateEffect>("Dilate", "An effect that applies dilation", "EyePlus");
            RegisterLayerEffectDescriptor<OpacityEffect>("Opacity", "A layer effect letting you change the opacity of all children", "Opacity");
            RegisterLayerEffectDescriptor<ErodeEffect>("Erode", "A layer effect providing an erode filter effect", "EyeMinus");
            RegisterLayerEffectDescriptor<GlowEffect>("Glow", "A layer effect providing a glow filter effect", "BoxShadow");
            RegisterLayerEffectDescriptor<GrayScaleEffect>("Gray-scale", "A layer effect providing a gray-scale filter effect", "InvertColors");
            RegisterLayerEffectDescriptor<ColorAdjustmentEffect>("Color adjustment", "A layer effect that lets you adjust the hue, brightness, contrast or saturation", "Tune");
            RegisterLayerEffectDescriptor<ColorMatrixEffect>("Color matrix", "A layer effect allowing you to apply a custom color matrix", "Matrix");
        }

        public override void Disable()
        {
        }
    }
}