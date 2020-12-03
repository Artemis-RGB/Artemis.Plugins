﻿using System.ComponentModel;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
    public class ColorBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Name = "Type", Description = "The type of color brush to draw")]
        public EnumLayerProperty<ColorType> GradientType { get; set; }

        [PropertyDescription(Description = "The color of the brush")]
        public SKColorLayerProperty Color { get; set; }

        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }

        [PropertyDescription(Description = "Change the rotation of the linear gradient without affecting the rotation of the shape", InputAffix = "°")]
        public FloatLayerProperty LinearGradientRotation { get; set; }

        [PropertyGroupDescription(Description = "Advanced radial gradient controls")]
        public RadialGradientProperties RadialGradient { get; set; }

        protected override void PopulateDefaults()
        {
            GradientType.DefaultValue = ColorType.Solid;
            Color.DefaultValue = new SKColor(255, 0, 0);
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
            ColorsMultiplier.DefaultValue = 0;
        }

        protected override void EnableProperties()
        {
            GradientType.CurrentValueSet += GradientTypeOnCurrentValueSet;
            UpdateVisibility();
        }

        protected override void DisableProperties()
        {
            GradientType.CurrentValueSet -= GradientTypeOnCurrentValueSet;
        }

        private void GradientTypeOnCurrentValueSet(object sender, LayerPropertyEventArgs<ColorType> e)
        {
            UpdateVisibility();
        }


        private void UpdateVisibility()
        {
            // Solid settings
            Color.IsHidden = GradientType.BaseValue != ColorType.Solid;

            // Gradients settings
            Colors.IsHidden = GradientType.BaseValue == ColorType.Solid;
            ColorsMultiplier.IsHidden = GradientType.BaseValue == ColorType.Solid;

            // Linear-gradient settings
            LinearGradientRotation.IsHidden = GradientType.BaseValue != ColorType.LinearGradient;
            RadialGradient.IsHidden = GradientType.BaseValue != ColorType.RadialGradient;
        }

        public enum ColorType
        {
            [Description("Solid")] Solid,

            [Description("Linear Gradient")] LinearGradient,

            [Description("Radial Gradient")] RadialGradient,

            [Description("Sweep Gradient")] SweepGradient
        }
    }
}