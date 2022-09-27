using System.Collections.Generic;
using WootingAnalogSDKNET;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Wooting.Services;

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
