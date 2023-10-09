using System.IO;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Wooting.WootingDeviceProvider;

namespace Artemis.Plugins.Devices.Wooting
{
    // ReSharper disable once UnusedMember.Global
    public class WootingDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public WootingDeviceProvider(ILogger logger, IRgbService rgbService)
        {
            _logger = logger;
            _rgbService = rgbService;
            CanDetectPhysicalLayout = true;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RGBDeviceProvider.PossibleX64NativePathsWindows.Add(Path.Combine(Plugin.Directory.FullName, "x64", "wooting-rgb-sdk64.dll"));
            RGBDeviceProvider.PossibleX86NativePathsWindows.Add(Path.Combine(Plugin.Directory.FullName, "x86", "wooting-rgb-sdk.dll"));
            RGBDeviceProvider.PossibleNativePathsLinux.Add(Path.Combine(Plugin.Directory.FullName, "x64", "libwooting-rgb-sdk.so"));

            RgbDeviceProvider.Exception += Provider_OnException;
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Wooting Exception: {message}", args.Exception.Message);
    }
}