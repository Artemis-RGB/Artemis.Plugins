using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using RGB.NET.Core;
using RGB.NET.Devices.SteelSeries;
using Serilog;
using Serilog.Events;

namespace Artemis.Plugins.Devices.SteelSeries
{
    // ReSharper disable once UnusedMember.Global
    public class SteelSeriesDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x1038;
        private readonly IRgbService _rgbService;
        private readonly ILogger _logger;

        public SteelSeriesDeviceProvider(IRgbService rgbService, ILogger logger) : base(RGB.NET.Devices.SteelSeries.SteelSeriesDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;
        }

        public override void Enable()
        {
            PathHelper.ResolvingAbsolutePath += (sender, args) => ResolveAbsolutePath(typeof(SteelSeriesRGBDevice), sender, args);
            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();
        }

        private void LogDeviceIds()
        {
            List<HidDevice> devices = DeviceList.Local.GetHidDevices(VENDOR_ID).DistinctBy(d => d.ProductID).ToList();
            _logger.Debug("Found {count} SteelSeries device(s)", devices.Count);
            foreach (HidDevice hidDevice in devices)
            {
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
}