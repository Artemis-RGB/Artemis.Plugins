using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.Devices.Msi
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "MSI Device Provider")]
    public class MsiDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public MsiDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGB.NET.Devices.Msi.MsiDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "MysticLight_SDK.dll"));
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "MysticLight_SDK.dll"));

            RGB.NET.Devices.Msi.MsiDeviceProvider.Instance.Exception += Provider_OnException;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGB.NET.Devices.Msi.MsiDeviceProvider.Instance.Exception -= Provider_OnException;
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "MSI Exception: {message}", args.Exception.Message);
    }
}