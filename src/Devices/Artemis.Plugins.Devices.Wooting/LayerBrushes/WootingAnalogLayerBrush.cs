using Artemis.Core;
using Artemis.Core.LayerBrushes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Wooting.LayerBrushes
{
    internal class WootingAnalogLayerBrush : PerLedLayerBrush<WootingAnalogPropertyGroup>
    {
        private readonly WootingAnalogService _service;

        public WootingAnalogLayerBrush(WootingAnalogService service)
        {
            _service = service;
        }

        public override void DisableLayerBrush()
        {
            
        }

        public override void EnableLayerBrush()
        {
            
        }

        public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
        {
            if (_service.Values.TryGetValue(led.RgbLed.Id, out var percent))
            {
                return Properties.Color.CurrentValue.GetColor(percent);
            }

            return SKColors.Empty;
        }

        public override void Update(double deltaTime)
        {

        }
    }
}
