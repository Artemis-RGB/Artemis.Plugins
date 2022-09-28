using System;
using Artemis.Core;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Corsair.Extensions;

public static class DeviceExtensions
{
    public static bool IsEarlyK95Platinum(this ArtemisDevice device)
    {
        // The model name must match and the G6 key should be above the G1 key
        return device.RgbDevice.DeviceInfo.Model.Equals("K95 RGB PLATINUM", StringComparison.InvariantCultureIgnoreCase) &&
               device.RgbDevice[LedId.Keyboard_Programmable6]!.Location.Y < device.RgbDevice[LedId.Keyboard_Programmable1]!.Location.Y;
    }
}