using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class RadialGradientProperties : LayerPropertyGroup
    {
        public enum RadialGradientResizeMode
        {
            Fit,
            Fill,
            Stretch
        }

        [PropertyDescription(Name = "Center offset", Description = "Change the position of the gradient by offsetting it from the center of the layer", InputAffix = "%")]
        public SKPointLayerProperty CenterOffset { get; set; }

        [PropertyDescription(Name = "Resize mode", Description = "How to make the gradient adjust to scale changes")]
        public EnumLayerProperty<RadialGradientResizeMode> ResizeMode { get; set; }

        protected override void PopulateDefaults()
        {
            ResizeMode.DefaultValue = RadialGradientResizeMode.Fit;
        }

        protected override void EnableProperties()
        {
            UpdateVisibility();
        }

        protected override void DisableProperties()
        {
        }

        private void UpdateVisibility()
        {
        }
    }
}