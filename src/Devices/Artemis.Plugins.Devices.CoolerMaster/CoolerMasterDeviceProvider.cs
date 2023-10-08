using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.CoolerMaster.CoolerMasterDeviceProvider;

namespace Artemis.Plugins.Devices.CoolerMaster
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "CoolerMaster Device Provider")]
    public class CoolerMasterDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;

        public CoolerMasterDeviceProvider(ILogger logger, IDeviceService deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RgbDeviceProvider.Exception += Provider_OnException;
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "CMSDK.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "CMSDK.dll"));
            _deviceService.AddDeviceProvider(this);
        }

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Cooler Master Exception: {message}", args.Exception.Message);
    }
}