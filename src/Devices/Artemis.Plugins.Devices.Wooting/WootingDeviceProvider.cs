using System.IO;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.Devices.Wooting
{
    // ReSharper disable once UnusedMember.Global
    public class WootingDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public WootingDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGB.NET.Devices.Wooting.WootingDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
            CanDetectPhysicalLayout = true;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "wooting-rgb-sdk64.dll"));
            RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "wooting-rgb-sdk.dll"));
            RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX64NativePathsLinux.Add(Path.Combine(Plugin.Directory.FullName, "x64", "libwooting-rgb-sdk.so"));

            RGB.NET.Devices.Wooting.WootingDeviceProvider.Instance.Exception += Provider_OnException;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGB.NET.Devices.Wooting.WootingDeviceProvider.Instance.Exception -= Provider_OnException;
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Wooting Exception: {message}", args.Exception.Message);
    }
}