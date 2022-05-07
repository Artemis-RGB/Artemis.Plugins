using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using Microsoft.Win32;
using Serilog;
using Serilog.Events;

namespace Artemis.Plugins.Devices.Logitech
{
    public class LogitechDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x046D;
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;
        private bool _suspended;

        public LogitechDeviceProvider(IRgbService rgbService, ILogger logger) : base(RGB.NET.Devices.Logitech.LogitechDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Logitech.LogitechDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "LogitechLedEnginesWrapper.dll"));
            RGB.NET.Devices.Logitech.LogitechDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "LogitechLedEnginesWrapper.dll"));

            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();

            SystemEvents.PowerModeChanged += SystemEventsPowerModeChanged;
            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }

        private void LogDeviceIds()
        {
            List<HidDevice> devices = DeviceList.Local.GetHidDevices(VENDOR_ID).DistinctBy(d => d.ProductID).ToList();
            _logger.Debug("Found {count} Logitech device(s)", devices.Count);
            foreach (HidDevice hidDevice in devices)
            {
                try
                {
                    _logger.Debug("Found Logitech device {name} with PID 0x{pid}", hidDevice.GetFriendlyName(), hidDevice.ProductID.ToString("X"));
                }
                catch (Exception)
                {
                    _logger.Debug("Found Logitech device with PID 0x{pid}", hidDevice.ProductID.ToString("X"));
                }
            }
        }

        #region Event handlers

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (e.Reason == SessionSwitchReason.SessionUnlock && _suspended)
                Task.Run(() =>
                {
                    // Give LGS a moment to think about its sins
                    Thread.Sleep(5000);
                    _logger.Debug("Detected PC unlock and we're suspended, calling Enable");
                    _suspended = false;
                    Enable();
                });
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