using System;
using Artemis.Core.LayerEffects;
using Artemis.Plugins.LayerEffects.Filter.Utilities;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class ColorAdjustmentEffect : LayerEffect<ColorAdjustmentEffectProperties>
    {
        public override void EnableLayerEffect()
        {
        }

        public override void DisableLayerEffect()
        {
        }

        public override void Update(double deltaTime)
        {
        }

        public override void PreProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            SKColorFilter filter;
            switch (Properties.Type.CurrentValue)
            {
                case ColorAdjustmentType.Hue:
                    filter = ColorFilterGenerator.GetHueColorMatrix(Properties.HueRotation);
                    break;
                case ColorAdjustmentType.Brightness:
                    filter = ColorFilterGenerator.GetBrightnessColorMatrix(Properties.Strength);
                    break;
                case ColorAdjustmentType.Contrast:
                    filter = ColorFilterGenerator.GetContrastColorMatrix(Properties.Strength);
                    break;
                case ColorAdjustmentType.Saturation:
                    filter = ColorFilterGenerator.GetSaturationSaturationMatrix(Properties.Strength);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            paint.ImageFilter = SKImageFilter.CreateColorFilter(filter, paint.ImageFilter);
        }

        public override void PostProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
        }
    }
}