using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.PhilipsHue.Models;
using Artemis.Plugins.PhilipsHue.RGB.NET;
using Artemis.Plugins.PhilipsHue.RGB.NET.Hue;
using Artemis.Plugins.PhilipsHue.Services;

namespace Artemis.Plugins.PhilipsHue
{
    public class HueDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;
        private readonly IHueService _hueService;

        public HueDeviceProvider(IRgbService rgbService, IHueService hueService) : base(HueRGBDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _hueService = hueService;
        }

        public override void Enable()
        {
            HueRGBDeviceProvider.Instance.ClientDefinitions.Clear();
            foreach (PhilipsHueBridge bridge in _hueService.Bridges)
                HueRGBDeviceProvider.Instance.ClientDefinitions.Add(new HueClientDefinition(bridge.IpAddress, bridge.AppKey, bridge.StreamingClientKey));

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }
    }
}