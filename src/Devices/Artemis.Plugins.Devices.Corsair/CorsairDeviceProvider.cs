using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Corsair.Extensions;
using Microsoft.Win32;
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

                Subscribe();
            }
            catch (CUEException e)
            {
                throw new ArtemisPluginException($"Corsair SDK threw error: {e.Error}", e);
            }
        }

        public override void Disable()
        {
            Unsubscribe();
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

        #region Event handlers

        private void Subscribe()
        {
            Thread thread = new(() =>
            {
                try
                {
                    SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
                }
                catch (Exception e)
                {
                    _logger.Warning(e, "Could not subscribe to SessionSwitch");
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void Unsubscribe()
        {
            Thread thread = new(() => SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (!IsEnabled || e.Reason != SessionSwitchReason.SessionUnlock)
                return;

            Task.Run(async () =>
            {
                Process icue = Process.GetProcessesByName("iCUE").FirstOrDefault();
                string path = icue?.MainModule?.FileName;
                if (path == null)
                    return;

                // Disable the plugin
                _logger.Debug("Detected PC unlock, restarting iCUE and reloading Corsair plugin");
                _pluginManagementService.DisablePlugin(_plugin, false);

                // Kill iCUE
                icue.Kill();

                // Restart iCUE
                Process.Start(path, "--autorun");

                // It takes about 8 seconds on my system but enable the plugin with the management service, allowing retries 
                await Task.Delay(8000);
                _logger.Debug("Re-enabling Corsair device provider");
                _pluginManagementService.EnablePlugin(_plugin, false);
            });
        }

        #endregion
    }
}