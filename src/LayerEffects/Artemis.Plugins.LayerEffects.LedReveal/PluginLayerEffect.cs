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
        private SKPath _clipPath;
        private int _lastToReveal;
        private int _lastMaxVisible;

        public override void EnableLayerEffect()
        {
        }

        public override void DisableLayerEffect()
        {
            _clipPath?.Dispose();
            _clipPath = null;
        }

        public override void Update(double deltaTime)
        {
        }

        public override void PreProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            // TODO: Lets see whether we can tell Artemis we only support layers
            if (ProfileElement is not Layer layer || !layer.Leds.Any())
                return;

            // Find out how many LEDs to reveal according to the current percentage
            int toReveal = Properties.RoundingFunction.CurrentValue switch
            {
                RoundingFunction.Round => (int) Math.Round(layer.Leds.Count / 100.0 * Math.Min(100, Properties.Percentage.CurrentValue), MidpointRounding.AwayFromZero),
                RoundingFunction.Floor => (int) Math.Floor(layer.Leds.Count / 100.0 * Math.Min(100, Properties.Percentage.CurrentValue)),
                RoundingFunction.Ceiling => (int) Math.Ceiling(layer.Leds.Count / 100.0 * Math.Min(100, Properties.Percentage.CurrentValue)),
                _ => throw new ArgumentOutOfRangeException()
            };

            // If the amount hasn't changed, reuse the last path
            if (toReveal == _lastToReveal && Properties.MaxVisibleLeds == _lastMaxVisible && _clipPath != null)
            {
                canvas.ClipPath(_clipPath);
                return;
            }

            // Order LEDs by their position to create a nice revealing effect from left top right, top to bottom
            List<ArtemisLed> leds = layer.Leds.OrderBy(l => l.AbsoluteRectangle.Top).ThenBy(l => l.AbsoluteRectangle.Left).ToList();
            // Because rendering for effects is 0,0 based, zero out the position of LEDs starting at the top-left
            float offsetX = leds.First().AbsoluteRectangle.Left;
            float offsetY = leds.First().AbsoluteRectangle.Top;

            // Create or reset the path
            if (_clipPath == null)
                _clipPath = new SKPath();
            else
                _clipPath.Reset();

            IEnumerable<ArtemisLed> ledsEnumerable = leds.Take(toReveal);
            if (Properties.LimitVisibleLeds)
                ledsEnumerable = ledsEnumerable.Skip(toReveal - Properties.MaxVisibleLeds);
            foreach (ArtemisLed artemisLed in ledsEnumerable)
            {
                _clipPath.AddRect(SKRect.Create(
                    artemisLed.AbsoluteRectangle.Left - offsetX,
                    artemisLed.AbsoluteRectangle.Top - offsetY,
                    artemisLed.AbsoluteRectangle.Width,
                    artemisLed.AbsoluteRectangle.Height));
            }

            canvas.ClipPath(_clipPath);
            _lastMaxVisible = Properties.MaxVisibleLeds;
            _lastToReveal = toReveal;
        }

        public override void PostProcess(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
        }
    }
}