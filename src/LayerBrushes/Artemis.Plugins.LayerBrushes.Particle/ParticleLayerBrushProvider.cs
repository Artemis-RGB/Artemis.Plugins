using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Particle
{
    // This is your plugin feature, it provides Artemis with one or more layer effects via descriptors.
    // Your feature gets enabled once. Your layer effects get enabled multiple times, once for each profile element (folder/layer) it is applied to.
    public class ParticleLayerBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            // This is where we can register our effect for use, we can also register multiple effects if we'd like
            RegisterLayerBrushDescriptor<ParticleLayerBrush>("Particles", "Renders configurable particles", "Meteor");
        }

        public override void Disable()
        {
            // Any registrations we made will be removed automatically, we don't need to do anything here
        }
    }
}