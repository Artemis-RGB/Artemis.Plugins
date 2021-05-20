using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups
{
    public class AmbilightPropertyGroup : LayerPropertyGroup
    {
        #region Properties & Fields

        public AmbilightCaptureProperties Capture { get; set; }

        #endregion

        #region Methods

        protected override void PopulateDefaults()
        {
        }

        protected override void EnableProperties()
        {
            Capture.IsHidden = true;
        }

        protected override void DisableProperties()
        {
        }

        #endregion
    }
}