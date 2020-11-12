using Artemis.Core.DeviceProviders;
using Artemis.Plugins.PhilipsHue.RGB.NET;
using Artemis.Plugins.PhilipsHue.Services;

namespace Artemis.Plugins.PhilipsHue
{
    public class HueDeviceProvider : DeviceProvider
    {
        private readonly IHueService _hueService;

        public HueDeviceProvider(IHueService hueService) : base(HueRGBDeviceProvider.Instance)
        {
            _hueService = hueService;
        }

        public override void Enable()
        {
        }
    }
}