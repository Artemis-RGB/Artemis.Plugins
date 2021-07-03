using System;
using Artemis.Core;
using Artemis.Core.LayerEffects;
using Artemis.Plugins.LayerEffect.Strobe.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffect.Strobe
{
    // This is the layer effect, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class PluginLayerEffect : LayerEffect<MainPropertyGroup>
    {
        private float _progress;

        public override void EnableLayerEffect()
        {
        }

        public override void DisableLayerEffect()
        {
        }

        public override void Update(double deltaTime)
        {
            // Progress moves from 0 to 2 which is brought back to a value moving from 0 to 1 to 0 during pre-process
            _progress = Mod((float) (_progress + deltaTime * Properties.Speed * 2), 2);
        }

        public override void PreProcess(SKCanvas canvas, SKRect renderBounds, SKPaint paint)
        {
            // Progress moves from 0 to 2 bring that back to a value moving from 0 to 1 to 0
            float progress = _progress;
            if (progress > 1)
                progress = 2 - progress;

            float alphaMultiplier;
            if (_progress <= 1f)
            {
                if (Properties.BrightenTransitionMode.CurrentValue == StrobeTransitionMode.Linear)
                    alphaMultiplier = progress;
                else if (Properties.BrightenTransitionMode.CurrentValue == StrobeTransitionMode.Eased)
                    alphaMultiplier = (float) Easings.BounceEaseInOut(progress);
                else
                    alphaMultiplier = MathF.Round(progress, 0);
            }
            else
            {
                if (Properties.DimTransitionMode.CurrentValue == StrobeTransitionMode.Linear)
                    alphaMultiplier = progress;
                else if (Properties.DimTransitionMode.CurrentValue == StrobeTransitionMode.Eased)
                    alphaMultiplier = (float) Easings.BounceEaseInOut(progress);
                else
                    alphaMultiplier = MathF.Round(progress, 0);
            }

            if (Properties.Inverted == true)
                alphaMultiplier = 1 - alphaMultiplier;

            paint.ColorF = paint.ColorF.WithAlpha(paint.ColorF.Alpha * alphaMultiplier);
        }

        public override void PostProcess(SKCanvas canvas, SKRect renderBounds, SKPaint paint)
        {
        }

        private static float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}