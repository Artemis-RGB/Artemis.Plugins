using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Core.Services;
using Artemis.Plugins.Input.LayerBrush.Keypress.Effects;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress
{
    public class KeypressBrush : LayerBrush<KeypressBrushProperties>
    {
        private readonly IInputService _inputService;
        private readonly List<IKeyPressEffect> _effects;

        public Random Rand { get; set; }

        public KeypressBrush(IInputService inputService)
        {
            _inputService = inputService;
            _effects = new List<IKeyPressEffect>();
        }

        public override void EnableLayerBrush()
        {
            Rand = new Random(Layer.EntityId.GetHashCode());

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
            // Ignore negative updates coming from the editor, no negativity here!
            if (deltaTime <= 0)
                return;

            lock (_effects)
            {
                _effects.RemoveAll(w => w.Finished);
                foreach (IKeyPressEffect keypressWave in _effects)
                    keypressWave.Update(deltaTime);
            }
        }


        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            lock (_effects)
            {
                foreach (IKeyPressEffect effect in _effects)
                    effect.Render(canvas);
            }
        }

        private void SpawnEffect(ArtemisLed led, SKPoint relativeLedPosition)
        {
            lock (_effects)
            {
                List<IKeyPressEffect> effects = _effects.Where(w => w.Led == led).ToList();
                foreach (IKeyPressEffect effect in effects)
                {
                    effect.Respawn();
                }

                // Create another one
                if (!effects.Any() || effects.All(e => e.AllowDuplicates))
                {
                    switch (Properties.Animation.CurrentValue)
                    {
                        case AnimationType.CircleWhilePressed:
                            _effects.Add(new KeypressWhilePressed(this, led, relativeLedPosition));
                            break;
                        case AnimationType.Ripple:
                             _effects.Add(new KeypressRipple(this, led, relativeLedPosition));
                             break;
                        case AnimationType.Echo:
                            _effects.Add(new KeyPressEcho(this, led, relativeLedPosition));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void DespawnWave(ArtemisLed led)
        {
            lock (_effects)
            {
                List<IKeyPressEffect> effects = _effects.Where(w => w.Led == led).ToList();
                foreach (IKeyPressEffect effect in effects)
                    effect.Despawn();
            }
        }

        #region Event handlers

        private void InputServiceOnKeyboardKeyDown(object sender, ArtemisKeyboardKeyEventArgs e)
        {
            if (e.Led == null)
                return;

            // Get the position of the LED relative to the layer
            SKPoint relativeLedPosition = new SKPoint(e.Led.AbsoluteRectangle.MidX - Layer.Bounds.Left, e.Led.AbsoluteRectangle.MidY - Layer.Bounds.Top);
            SpawnEffect(e.Led, relativeLedPosition);
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