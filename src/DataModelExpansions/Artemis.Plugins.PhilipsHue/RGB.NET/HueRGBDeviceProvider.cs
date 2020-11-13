using System;
using System.Collections.Generic;
using System.Text;
using RGB.NET.Core;

namespace Artemis.Plugins.PhilipsHue.RGB.NET
{
    public class HueRGBDeviceProvider : IRGBDeviceProvider
    {
        private static HueRGBDeviceProvider _instance;
        public static HueRGBDeviceProvider Instance => _instance ?? new HueRGBDeviceProvider();

        public bool IsInitialized { get; private set; }
        public IEnumerable<IRGBDevice> Devices { get; private set; }
        public bool HasExclusiveAccess { get; private set; }

        public bool Initialize(RGBDeviceType loadFilter = RGBDeviceType.All, bool exclusiveAccessIfPossible = false, bool throwExceptions = false)
        {
            try
            {
                // Initialize your device here
            }
            catch
            {
                if (throwExceptions) throw;
                return false;
            }

            return true;
        }

        public void Dispose()
        {
        }

        public void ResetDevices()
        {
        }
    }
}
