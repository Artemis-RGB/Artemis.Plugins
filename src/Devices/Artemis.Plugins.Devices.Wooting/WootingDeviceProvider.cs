using System.IO;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.Wooting
{
    // ReSharper disable once UnusedMember.Global
    public class WootingDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public WootingDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Wooting.WootingDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            CanDetectPhysicalLayout = true;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "wooting-rgb-sdk64.dll"));
            RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "wooting-rgb-sdk.dll"));
            
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }
    }
}