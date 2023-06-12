using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.Devices.Wooting.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Plugins.Devices.Wooting.Services.AnalogService;

namespace Artemis.Plugins.Devices.Wooting.LayerBrushes;

internal class WootingAnalogLayerBrush : PerLedLayerBrush<WootingAnalogPropertyGroup>
{
    private readonly WootingAnalogService _analogService;
    private int _useToken;

    public WootingAnalogLayerBrush(WootingAnalogService service)
    {
        _analogService = service;
    }
    
    public override void EnableLayerBrush()
    { 
        _useToken = _analogService.RegisterUse();
    }

    public override void DisableLayerBrush()
    {
        _analogService.UnregisterUse(_useToken);
    }

    public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
    {
        string deviceName = led.Device.RgbDevice.DeviceInfo.Model;
        if (!WootingModelNameDictionary.WootingModelNames.TryGetValue(deviceName, out string wootingDeviceName))
            return SKColors.Empty;
        
        WootingAnalogDevice analogDevice = _analogService.Devices.FirstOrDefault(d => d.Info.device_name == wootingDeviceName);
        if (analogDevice == null)
            return SKColor.Empty;

        if (!analogDevice.AnalogValues.TryGetValue(led.RgbLed.Id, out float percent)) 
            return SKColors.Empty;
        
        return Properties.Color.CurrentValue.GetColor(percent);
    }

    public override void Update(double deltaTime)
    {
        _analogService.Update();
    }
}
