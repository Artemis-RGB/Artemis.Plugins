using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Corsair.Extensions;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.CorsairLegacy.CorsairLegacyDeviceProvider;

namespace Artemis.Plugins.Devices.Corsair
{
    // ReSharper disable once UnusedMember.Global
    public class CorsairLegacyDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private readonly Plugin _plugin;

        public CorsairLegacyDeviceProvider(ILogger logger, IDeviceService deviceService, Plugin plugin)
        {
            _logger = logger;
            _deviceService = deviceService;
            _plugin = plugin;

            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;
            CreateMissingLedsSupported = false;
            SuspendSupported = true;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            if (_plugin.GetFeature<CorsairDeviceProvider>()!.IsEnabled)
                throw new ArtemisPluginException("The legacy Corsair device provider cannot be enabled while the new Corsair device provider is enabled");
            
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "CUESDK.x64_2019.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "CUESDK_2019.dll"));
            try
            {
                RGBDeviceProvider.Instance.Exception += Provider_OnException;

                _deviceService.AddDeviceProvider(this);
                
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

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Corsair Exception: {message}", args.Exception.Message);

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);

            RGBDeviceProvider.Instance.Exception -= Provider_OnException;
            RGBDeviceProvider.Instance.Dispose();
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

        public override string GetDeviceLayoutName(ArtemisDevice device)
        {
            // Pumps can have different amounts of LEDs but share the model name "PUMP", by including the LED count in the layout name
            // we can still have different layouts of each type of pump
            if (device.RgbDevice.DeviceInfo.Model.Equals("pump", StringComparison.InvariantCultureIgnoreCase))
                return $"PUMP-{device.RgbDevice.Count()}-ZONE.xml";

            // Early K95 Platinum models used the first key of columns of 3, use alternative layout
            if (device.IsEarlyK95Platinum())
                return $"K95 RGB PLATINUM-ALT-{device.PhysicalLayout.ToString().ToUpper()}.xml";

            return base.GetDeviceLayoutName(device);
        }
        
        public override Task Suspend()
        {
            RGBDeviceProvider.Instance.Dispose();
            return Task.CompletedTask;
        }

        public override async Task Resume()
        {
            Process icue = Process.GetProcessesByName("iCUE").FirstOrDefault();
            string path = icue?.MainModule?.FileName;
            if (path == null)
                return;

            // Kill iCUE
            icue.Kill();

            // Restart iCUE
            Process.Start(path, "--autorun");

            // It takes about 8 seconds on my system but enable the plugin with the management service, allowing retries 
            await Task.Delay(8000);
        }
    }
}