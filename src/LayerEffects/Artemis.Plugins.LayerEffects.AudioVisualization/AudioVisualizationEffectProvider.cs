using Artemis.Core;
using Artemis.Core.LayerEffects;

namespace Artemis.Plugins.LayerEffects.AudioVisualization
{
    [PluginFeature(AlwaysEnabled = true)]
    public class AudioVisualizationEffectProvider : LayerEffectProvider
    {
        #region Methods

        public override void Enable()
        {
            RegisterLayerEffectDescriptor<AudioVisualizationEffect>("Audio Visualization", "Super awesome audio visualization", "Music");
        }

        public override void Disable()
        {
        }

        #endregion
    }
}