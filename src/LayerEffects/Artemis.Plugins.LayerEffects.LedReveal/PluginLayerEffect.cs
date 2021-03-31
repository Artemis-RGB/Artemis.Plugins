using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerEffects;
using Artemis.Plugins.LayerEffects.LedReveal.PropertyGroups;
using SkiaSharp;


namespace Artemis.Plugins.LayerEffects.LedReveal
{
    // This is the layer effect, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class PluginLayerEffect : LayerEffect<MainPropertyGroup>
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
        }

        public override void PostProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (ProfileElement is Layer layer)
            {
                List<ArtemisLed> leds = layer.Leds.OrderBy(l => l.AbsoluteRectangle.Top).ThenBy(l => l.AbsoluteRectangle.Left).ToList();
                if (!leds.Any())
                    return;

                float offsetX = leds.First().AbsoluteRectangle.Left;
                float offsetY = leds.First().AbsoluteRectangle.Top;
                float toReveal = leds.Count / 100f * Math.Min(100f, Properties.Percentage.CurrentValue);

                using SKPaint overlayPaint = new() {Color = SKColors.Black};
                for (int index = 0; index < leds.Count; index++)
                {
                    if (index + 1 < toReveal && !(!Properties.ShowAllRevealedLeds && index + 1 < toReveal - Properties.MaxShownLeds))
                        continue;

                    ArtemisLed artemisLed = leds[index];
                    canvas.DrawRect(
                        artemisLed.AbsoluteRectangle.Left - offsetX,
                        artemisLed.AbsoluteRectangle.Top - offsetY,
                        artemisLed.AbsoluteRectangle.Width,
                        artemisLed.AbsoluteRectangle.Height,
                        overlayPaint
                    );
                }
            }
        }
    }
}