using Artemis.Core.LayerEffects;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class OpacityEffect : LayerEffect<OpacityEffectProperties>
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
            paint.Color = paint.Color.WithAlpha((byte) (Properties.Opacity.CurrentValue * 2.55f));
        }

        public override void PostProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
        }
    }
}