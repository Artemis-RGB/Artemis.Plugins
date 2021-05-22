using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightLayerBrushProvider : LayerBrushProvider
    {
        #region Methods

        public override void Enable()
        {
            RegisterLayerBrushDescriptor<AmbilightLayerBrush>("Ambilight", "A brush that shows the current display-image.", "MonitorMultiple");
        }

        public override void Disable()
        { }

        #endregion
    }
}
