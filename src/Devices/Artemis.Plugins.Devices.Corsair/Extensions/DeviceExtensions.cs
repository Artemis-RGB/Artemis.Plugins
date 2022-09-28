using System;
using Artemis.Core;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Corsair.Extensions;

public static class DeviceExtensions
{
    // ReSharper disable once InconsistentNaming
    public static bool IsEarlyK95PlatinumXT(this ArtemisDevice device)
    {
        // The model name must match and the G6 key should be above the G1 key
        return device.RgbDevice.DeviceInfo.Model.Equals("K95 RGB PLATINUM XT", StringComparison.InvariantCultureIgnoreCase) &&
               device.RgbDevice[LedId.Keyboard_Programmable6]!.Location.Y < device.RgbDevice[LedId.Keyboard_Programmable1]!.Location.Y;
    }
}