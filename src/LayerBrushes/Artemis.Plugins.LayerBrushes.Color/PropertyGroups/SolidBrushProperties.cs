using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class SolidBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "Whether or not to animate between multiple colors")]
        public BoolLayerProperty EnableColorAnimation { get; set; }

        [PropertyDescription(Description = "The color of the brush")]
        public SKColorLayerProperty Color { get; set; }

        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Description = "The speed at which the brush moves between the different colors")]
        public FloatLayerProperty AnimationSpeed { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Color.DefaultValue = new SKColor(255, 0, 0);
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            AnimationSpeed.DefaultValue = 100;
        }

        protected override void EnableProperties()
        {
            EnableColorAnimation.CurrentValueSet += EnableColorAnimationOnCurrentValueSet;
        }

        protected override void DisableProperties()
        {
            EnableColorAnimation.CurrentValueSet -= EnableColorAnimationOnCurrentValueSet;
        }


        private void EnableColorAnimationOnCurrentValueSet(object sender, LayerPropertyEventArgs<bool> e)
        {
            Color.IsHidden = EnableColorAnimation.CurrentValue;
            Colors.IsHidden = !EnableColorAnimation.CurrentValue;
        }

        #endregion
    }

    public enum SolidColorMode
    {
        Single,
        Multiple
    }
}