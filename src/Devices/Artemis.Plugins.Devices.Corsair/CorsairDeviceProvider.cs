using System;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Corsair.Extensions;
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
        private readonly IPluginManagementService _pluginManagementService;
        private readonly Plugin _plugin;

        public CorsairDeviceProvider(ILogger logger, IRgbService rgbService, IPluginManagementService pluginManagementService, Plugin plugin) : base(RGBDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
            _pluginManagementService = pluginManagementService;
            _plugin = plugin;

            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;
            CreateMissingLedsSupported = false;
        }

        public override void Enable()
        {
            if (_plugin.GetFeature<CorsairLegacyDeviceProvider>()!.IsEnabled)
                throw new ArtemisPluginException("The new Corsair device provider cannot be enabled while the legacy Corsair device provider is enabled");
            
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "iCUESDK.x64_2019.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "iCUESDK_2019.dll"));
            try
            {
                RGBDeviceProvider.Instance.SessionStateChanged += SessionStateChanged;
                RGBDeviceProvider.Instance.Exception += Provider_OnException;

                _rgbService.AddDeviceProvider(RgbDeviceProvider);

                _logger.Debug("Corsair SDK details");
                _logger.Debug(" - Client version: {detail}", RGBDeviceProvider.Instance.SessionDetails.ClientVersion);
                _logger.Debug(" - Server version: {detail}", RGBDeviceProvider.Instance.SessionDetails.ServerVersion);
                _logger.Debug(" - Server-Host version: {detail}", RGBDeviceProvider.Instance.SessionDetails.ServerHostVersion);
            }
            catch (CUEException e)
            {
                throw new ArtemisPluginException($"Corsair SDK threw error: {e.Error}", e);
            }
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Corsair Exception: {message}", args.Exception.Message);

        private void SessionStateChanged(object sender, CorsairSessionState state) => _logger.Debug("Corsair Session-State: {state}", state);

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGBDeviceProvider.Instance.SessionStateChanged -= SessionStateChanged;
            RGBDeviceProvider.Instance.Exception -= Provider_OnException;
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
    }
}