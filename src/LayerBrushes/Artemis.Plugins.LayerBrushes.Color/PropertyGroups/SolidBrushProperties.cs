using System;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class SolidBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The color mode, allowing you to either have a static color or a color from a gradient")]
        public EnumLayerProperty<SolidBrushColorMode> ColorMode { get; set; }

        [PropertyDescription(Description = "The color of the brush")]
        public SKColorLayerProperty Color { get; set; }

        [PropertyDescription(Name = "Gradient", Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Description = "The position at which the color is taken from the gradient", InputAffix = "%", MinInputValue = 0f, MaxInputValue = 100f)]
        public FloatLayerProperty GradientPosition { get; set; }

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
            Color.IsVisibleWhen(ColorMode, p => p.CurrentValue == SolidBrushColorMode.Static);
            Colors.IsHiddenWhen(ColorMode, p => p.CurrentValue == SolidBrushColorMode.Static);
            
            GradientPosition.IsVisibleWhen(ColorMode, p => p.CurrentValue == SolidBrushColorMode.GradientPosition);
            AnimationSpeed.IsVisibleWhen(ColorMode, p => p.CurrentValue == SolidBrushColorMode.GradientAnimation);
        }

        protected override void DisableProperties()
        {
        }

        #endregion
    }

    public enum SolidBrushColorMode
    {
        Static,
        GradientAnimation,
        GradientPosition
    }
}