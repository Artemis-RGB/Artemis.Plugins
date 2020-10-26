using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using RGB.NET.Devices.Asus;
namespace Artemis.Plugins.Devices.Asus
{
    // ReSharper disable once UnusedMember.Global
    public class AsusDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public AsusDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Asus.AsusDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void EnablePlugin()
        {
            PathHelper.ResolvingAbsolutePath += (sender, args) => ResolveAbsolutePath(typeof(AsusRGBDevice<>), sender, args);
            RGB.NET.Devices.Asus.AsusDeviceProvider.Instance.Initialize(RGBDeviceType.All, false, true);
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }
    }
}