using System.Collections.Generic;
using RGB.NET.Core;
using WootingAnalogSDKNET;

namespace Artemis.Plugins.Devices.Wooting.Services.AnalogService;

public class WootingAnalogDevice
{
    public Dictionary<LedId, float> AnalogValues { get; }
    public DeviceInfo Info { get; }

    public WootingAnalogDevice(DeviceInfo info)
    {
        Info = info;
        AnalogValues = new Dictionary<LedId, float>();
    }
}
