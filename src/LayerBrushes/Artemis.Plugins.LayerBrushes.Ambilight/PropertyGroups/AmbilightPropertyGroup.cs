using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups
{
    public class AmbilightPropertyGroup : LayerPropertyGroup
    {
        #region Properties & Fields

        [PropertyDescription(Description = "The display-region to capture", DisableKeyframes = true)]
        public LayerProperty<AmbilightCaptureProperties?> Capture { get; set; }

        #endregion

        #region Methods

        protected override void PopulateDefaults()
        {
            Capture.DefaultValue = null;
        }

        protected override void EnableProperties()
        { }

        protected override void DisableProperties()
        { }

        #endregion
    }
}