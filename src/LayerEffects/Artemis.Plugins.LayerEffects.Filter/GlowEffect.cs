using Artemis.Core.LayerEffects;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class GlowEffect : LayerEffect<GlowEffectProperties>
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
            paint.ImageFilter = SKImageFilter.CreateDropShadow(
                Properties.GlowOffset.CurrentValue.X,
                Properties.GlowOffset.CurrentValue.Y,
                Properties.GlowBlurAmount.CurrentValue.Width,
                Properties.GlowBlurAmount.CurrentValue.Height,
                Properties.GlowColor,
                paint.ImageFilter
            );
        }

        public override void PostProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
        }
    }
}