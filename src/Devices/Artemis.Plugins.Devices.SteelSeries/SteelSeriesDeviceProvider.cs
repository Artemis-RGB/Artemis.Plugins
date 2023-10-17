using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using RGB.NET.Core;
using Serilog;
using Serilog.Events;
using RGBDeviceProvider = RGB.NET.Devices.SteelSeries.SteelSeriesDeviceProvider;

namespace Artemis.Plugins.Devices.SteelSeries
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "SteelSeries Device Provider")]
    public class SteelSeriesDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x1038;
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;

        public SteelSeriesDeviceProvider(IDeviceService deviceService, ILogger logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;
        
        public override void Enable()
        {
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

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Steel Series Exception: {message}", args.Exception.Message);

        private void LogDeviceIds()
        {
            List<HidDevice> devices = DeviceList.Local.GetHidDevices(VENDOR_ID).DistinctBy(d => d.ProductID).ToList();
            _logger.Debug("Found {count} SteelSeries device(s)", devices.Count);
            foreach (HidDevice hidDevice in devices)
                try
                {
                    _logger.Debug("Found SteelSeries device {name} with PID 0x{pid}", hidDevice.GetFriendlyName(), hidDevice.ProductID.ToString("X"));
                }
                catch (Exception)
                {
                    _logger.Debug("Found SteelSeries device with PID 0x{pid}", hidDevice.ProductID.ToString("X"));
                }
        }
    }
}