using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class SolidBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The color of the brush")]
        public SKColorLayerProperty Color { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Color.DefaultValue = new SKColor(255, 0, 0);
        }


        protected override void EnableProperties()
        {
        }


        protected override void DisableProperties()
        {
        }

        #endregion
    }
}