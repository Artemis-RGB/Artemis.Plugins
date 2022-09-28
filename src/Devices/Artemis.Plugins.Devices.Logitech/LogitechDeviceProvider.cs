using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IPluginManagementService _pluginManagementService;
        private readonly IRgbService _rgbService;

        public LogitechDeviceProvider(IRgbService rgbService, ILogger logger, IPluginManagementService pluginManagementService) : base(RGB.NET.Devices.Logitech.LogitechDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;
            _pluginManagementService = pluginManagementService;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Logitech.LogitechDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "LogitechLedEnginesWrapper.dll"));
            RGB.NET.Devices.Logitech.LogitechDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "LogitechLedEnginesWrapper.dll"));

            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();

            Subscribe();
        }

        public override void Disable()
        {
            Unsubscribe();
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
                // Disable the plugin
                Disable();

                // Enable the plugin with the management service, allowing retries 
                await Task.Delay(5000);
                _pluginManagementService.EnablePluginFeature(this, false, true);
            });
        }

        #endregion
    }
}