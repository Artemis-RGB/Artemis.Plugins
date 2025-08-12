using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Wooting.Grpc.WootingGrpcDeviceProvider;

namespace Artemis.Plugins.Devices.Wooting
{
    // ReSharper disable once UnusedMember.Global
    public class WootingDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;

        public WootingDeviceProvider(ILogger logger, IDeviceService deviceService, IInputService inputService)
        {
            _logger = logger;
            _deviceService = deviceService;
            CanDetectPhysicalLayout = true;
            inputService.AddInputProvider(new WootingAnalogInputProvider(logger, inputService, deviceService));
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            // RGBDeviceProvider.PossibleX64NativePathsWindows.Add(Path.Combine(Plugin.Directory.FullName, "x64", "wooting-rgb-sdk64.dll"));
            // RGBDeviceProvider.PossibleX86NativePathsWindows.Add(Path.Combine(Plugin.Directory.FullName, "x86", "wooting-rgb-sdk.dll"));
            // RGBDeviceProvider.PossibleNativePathsLinux.Add(Path.Combine(Plugin.Directory.FullName, "x64", "libwooting-rgb-sdk.so"));

            RgbDeviceProvider.Exception += Provider_OnException;
            _deviceService.AddDeviceProvider(this);
        }

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Wooting Exception: {message}", args.Exception.Message);
    }
}