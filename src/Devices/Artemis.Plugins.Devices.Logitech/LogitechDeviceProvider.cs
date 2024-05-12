using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public override Task Suspend()
        {
            RgbDeviceProvider.Dispose();
            return Task.CompletedTask;
        }

        public override async Task Resume()
        {
            await RestartProcessIfFound("ghub");
            await RestartProcessIfFound("lgs");
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
        
        private async Task RestartProcessIfFound(string processName)
        {
            Process process = Process.GetProcessesByName(processName).FirstOrDefault();
            string path = process?.MainModule?.FileName;
            if (path == null)
                return;

            // Kill process
            process.Kill();

            // Restart process
            Process.Start(path, "--autorun");

            // It takes about 8 seconds on my system but enable the plugin with the management service, allowing retries 
            await Task.Delay(8000);
        }

    }
}