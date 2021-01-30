using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using RGB.NET.Devices.Msi;

namespace Artemis.Plugins.Devices.Msi
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "MSI Device Provider")]
    public class MsiDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public MsiDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Msi.MsiDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            PathHelper.ResolvingAbsolutePath += (sender, args) => ResolveAbsolutePath(typeof(MsiRGBDevice<>), sender, args);
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "MysticLight_SDK.dll"));
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "MysticLight_SDK.dll"));
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }
    }
}