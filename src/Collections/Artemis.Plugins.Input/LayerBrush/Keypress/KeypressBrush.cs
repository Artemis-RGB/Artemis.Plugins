using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Core.Services;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress
{
    public class KeypressBrush : LayerBrush<KeypressBrushProperties>
    {
        private readonly IInputService _inputService;
        private readonly List<KeypressWave> _waves;
        private Random _rand;

        public KeypressBrush(IInputService inputService)
        {
            _inputService = inputService;
            _waves = new List<KeypressWave>();
        }

        public override void EnableLayerBrush()
        {
            int hash = Layer.EntityId.GetHashCode();
            _rand = new Random(Layer.EntityId.GetHashCode());

            _inputService.KeyboardKeyDown += InputServiceOnKeyboardKeyDown;
            _inputService.KeyboardKeyUp += InputServiceOnKeyboardKeyUp;
        }

        public override void DisableLayerBrush()
        {
            _inputService.KeyboardKeyDown -= InputServiceOnKeyboardKeyDown;
            _inputService.KeyboardKeyUp -= InputServiceOnKeyboardKeyUp;
        }


        public override void Update(double deltaTime)
        {
            lock (_waves)
            {
                _waves.RemoveAll(w => w.Size < 0);
                foreach (KeypressWave keypressWave in _waves)
                    keypressWave.Update(deltaTime);
            }
        }


        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            lock (_waves)
            {
                foreach (KeypressWave keypressWave in _waves)
                    keypressWave.Render(canvas);
            }
        }

        private void SpawnWave(ArtemisLed led, SKPoint relativeLedPosition)
        {
            lock (_waves)
            {
                List<KeypressWave> existing = _waves.Where(w => w.Led == led).ToList();
                if (existing.Any())
                {
                    foreach (KeypressWave keypressWave in existing)
                    {
                        keypressWave.Shrink = false;
                        keypressWave.Size = Math.Max(keypressWave.Size, 0);
                    }

                    return;
                }

                KeypressWave wave = new KeypressWave(
                    led,
                    relativeLedPosition,
                    new SKPaint {Color = SKColor.FromHsv(_rand.Next(0, 360), 100, 100)}
                );
                _waves.Add(wave);
            }
        }

        private void DespawnWave(ArtemisLed led)
        {
            lock (_waves)
            {
                List<KeypressWave> waves = _waves.Where(w => w.Led == led).ToList();
                foreach (KeypressWave keypressWave in waves)
                    keypressWave.Shrink = true;
            }
        }

        #region Event handlers

        private void InputServiceOnKeyboardKeyDown(object sender, ArtemisKeyboardKeyEventArgs e)
        {
            if (e.Led == null)
                return;

            // Get the position of the LED relative to the layer
            SKPoint relativeLedPosition = new SKPoint(e.Led.AbsoluteRenderRectangle.MidX - Layer.Bounds.Left, e.Led.AbsoluteRenderRectangle.MidY - Layer.Bounds.Top);
            SpawnWave(e.Led, relativeLedPosition);
        }

        private void InputServiceOnKeyboardKeyUp(object sender, ArtemisKeyboardKeyEventArgs e)
        {
            if (e.Led == null)
                return;

            DespawnWave(e.Led);
        }

        #endregion
    }
}