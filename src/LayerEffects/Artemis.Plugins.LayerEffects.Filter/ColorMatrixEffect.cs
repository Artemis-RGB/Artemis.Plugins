using Artemis.Core.LayerEffects;
using Artemis.Plugins.LayerEffects.Filter.ViewModels;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class ColorMatrixEffect : LayerEffect<ColorMatrixEffectProperties>
    {
        public override void EnableLayerEffect()
        {
            ConfigurationDialog = new LayerEffectConfigurationDialog<ColorMatrixConfigurationViewModel>();
        }

        public override void DisableLayerEffect()
        {
        }

        public override void Update(double deltaTime)
        {
        }

        public override void PreProcess(SKCanvas canvas, SKPath renderBounds, SKPaint paint)
        {
        }

        public override void PostProcess(SKCanvas canvas, SKPath renderBounds, SKPaint paint)
        {
            paint.ImageFilter = SKImageFilter.CreateColorFilter(SKColorFilter.CreateColorMatrix(Properties.ColorMatrix.CurrentValue), paint.ImageFilter);
        }
    }
}