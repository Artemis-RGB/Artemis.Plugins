using Artemis.Plugins.LayerBrushes.Particle.PropertyGroups;
using Artemis.UI.Shared.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticlesConfigurationViewModel : BrushConfigurationViewModel
    {
        public ParticlesConfigurationViewModel(PluginLayerBrush particlesBrush) : base(particlesBrush)
        {
            ParticlesBrush = particlesBrush;
            Properties = ParticlesBrush.Properties;
        }

        public PluginLayerBrush ParticlesBrush { get; }
        public MainPropertyGroup Properties { get; }
    }
}