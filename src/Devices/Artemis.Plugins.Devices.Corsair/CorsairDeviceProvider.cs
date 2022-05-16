using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
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
        private bool _suspended;

        public CorsairDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGBDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;

            // iCUE becomes unavailable on power mode Suspend but only becomes available again after SessionUnlock
            SystemEvents.PowerModeChanged += SystemEventsPowerModeChanged;
            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
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

        public override string GetDeviceLayoutName(ArtemisDevice device)
        {
            // Pumps can have different amounts of LEDs but share the model name "PUMP", by including the LED count in the layout name
            // we can still have different layouts of each type of pump
            if (device.RgbDevice.DeviceInfo.Model.Equals("pump", StringComparison.InvariantCultureIgnoreCase))
                return $"PUMP-{device.RgbDevice.Count()}-ZONE.xml";

            return base.GetDeviceLayoutName(device);
        }

        #region Event handlers

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (e.Reason == SessionSwitchReason.SessionUnlock && _suspended)
            {
                Task.Run(() =>
                {
                    // Give iCUE a moment to think about its sins
                    Thread.Sleep(5000);
                    _logger.Debug("Detected PC unlock and we're suspended, calling Enable");
                    _suspended = false;
                    Enable();
                });
            }
        }

        private void SystemEventsPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (e.Mode == PowerModes.Suspend && !_suspended)
            {
                _logger.Debug("Detected power state change to Suspend, calling Disable");
                _suspended = true;
                Disable();
            }
        }

        #endregion
    }
}