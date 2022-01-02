using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.PicoPi
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "PicoPi Device Provider")]
    public class PicoPiDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public PicoPiDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.PicoPi.PicoPiDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }
    }
}