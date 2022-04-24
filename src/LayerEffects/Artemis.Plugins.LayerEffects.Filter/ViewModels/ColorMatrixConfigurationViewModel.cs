
using Artemis.UI.Shared.LayerEffects;

namespace Artemis.Plugins.LayerEffects.Filter.ViewModels
{
    public class ColorMatrixConfigurationViewModel : EffectConfigurationViewModel
    {
        public ColorMatrixConfigurationViewModel(ColorMatrixEffect layerEffect) : base(layerEffect)
        {
            Properties = layerEffect.Properties;
        }

        public ColorMatrixEffectProperties Properties { get; }
    }
}