using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Corsair.CorsairDeviceProvider;

namespace Artemis.Plugins.Devices.Corsair
{
    // ReSharper disable once UnusedMember.Global
    public class CorsairDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public CorsairDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGBDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;
        }

        public override void Enable()
        {
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "CUESDK.x64_2017.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "CUESDK_2017.dll"));
            try
            {
                _rgbService.AddDeviceProvider(RgbDeviceProvider);
                
                if (RGBDeviceProvider.Instance.ProtocolDetails == null) return;
                _logger.Debug("Corsair SDK details");
                _logger.Debug(" - SDK version: {detail}", RGBDeviceProvider.Instance.ProtocolDetails.SdkVersion);
                _logger.Debug(" - SDK protocol version: {detail}", RGBDeviceProvider.Instance.ProtocolDetails.SdkProtocolVersion);
                _logger.Debug(" - Server version: {detail}", RGBDeviceProvider.Instance.ProtocolDetails.ServerVersion);
                _logger.Debug(" - Server protocol version: {detail}", RGBDeviceProvider.Instance.ProtocolDetails.ServerProtocolVersion);
            }
            catch (CUEException e)
            {
                throw new ArtemisPluginException($"Corsair SDK threw error: {e.Error}", e);
            }
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }

        public override string GetLogicalLayout(IKeyboard keyboard)
        {
            if (keyboard.DeviceInfo is CorsairKeyboardRGBDeviceInfo keyboardInfo)
            {
                // Simply use two-letter country code
                if (keyboardInfo.LogicalLayout == CorsairLogicalKeyboardLayout.US_Int)
                    return "US";
                return keyboardInfo.LogicalLayout.ToString();
            }

            return null;
        }
    }
}