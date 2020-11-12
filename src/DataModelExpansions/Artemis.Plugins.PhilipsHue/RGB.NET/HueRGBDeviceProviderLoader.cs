using RGB.NET.Core;

namespace Artemis.Plugins.PhilipsHue.RGB.NET
{
    public class HueRGBDeviceProviderLoader : IRGBDeviceProviderLoader
    {
        public IRGBDeviceProvider GetDeviceProvider()
        {
            return HueRGBDeviceProvider.Instance;
        }

        public bool RequiresInitialization => false;
    }
}