using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using RGB.NET.Core;
using Serilog;
using Serilog.Events;
using RGBDeviceProvider = RGB.NET.Devices.Logitech.LogitechDeviceProvider;

namespace Artemis.Plugins.Devices.Logitech
{
    public class LogitechDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x046D;
        private static readonly string[] LogitechProcesses = ["lghub_agent", "lghub_system_tray", "lghub", "LCore"];
        
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;

        public LogitechDeviceProvider(IDeviceService deviceService, ILogger logger)
        {
            _deviceService = deviceService;
            _logger = logger;

            SuspendSupported = true;
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            SdkHelper.EnsureSdkAvailable(_logger);
            
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "LogitechLedEnginesWrapper.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "LogitechLedEnginesWrapper.dll"));

            RgbDeviceProvider.Exception += Provider_OnException;
            _deviceService.AddDeviceProvider(this);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();
        }

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        public override void Suspend()
        {
            // Kill all Logitech processes, during plugin Enable they will be restarted
            List<Process> processes = Process.GetProcesses().Where(p => LogitechProcesses.Contains(p.ProcessName)).ToList();
            foreach (Process process in processes)
                process.Kill();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Logitech Exception: {message}", args.Exception.Message);

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
    }
}