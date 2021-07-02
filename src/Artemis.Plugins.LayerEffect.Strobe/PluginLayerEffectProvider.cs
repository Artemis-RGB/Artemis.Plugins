using Artemis.Core.LayerEffects;

namespace Artemis.Plugins.LayerEffect.Strobe
{
    public class PluginLayerEffectProvider : LayerEffectProvider
    {
        public override void Enable()
        {
            RegisterLayerEffectDescriptor<PluginLayerEffect>("Strobe", "Provides a strobing effect", "SineWave");
        }

        public override void Disable()
        {
        }
    }
}