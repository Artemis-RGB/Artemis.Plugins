using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.Devices.Wooting.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Wooting.LayerBrushes;

internal class WootingAnalogLayerBrush : PerLedLayerBrush<WootingAnalogPropertyGroup>
{
    private readonly WootingAnalogService _analogService;

    public WootingAnalogLayerBrush(WootingAnalogService service)
    {
        _analogService = service;
    }

    public override void DisableLayerBrush()
    {
        
    }

    public override void EnableLayerBrush()
    {
        
    }

    public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
    {
        ArtemisDevice artemisDevice = led.Device;
        WootingAnalogDevice analogDevice = _analogService.Devices.FirstOrDefault(ad => ad.Info.device_name == artemisDevice.RgbDevice.DeviceInfo.Model.Replace(" ", ""));
        if (analogDevice == null)
            return SKColor.Empty;

        if (analogDevice.AnalogValues.TryGetValue(led.RgbLed.Id, out float percent))
            return Properties.Color.CurrentValue.GetColor(percent);

        return SKColors.Empty;
    }

    public override void Update(double deltaTime)
    {

    }
}
