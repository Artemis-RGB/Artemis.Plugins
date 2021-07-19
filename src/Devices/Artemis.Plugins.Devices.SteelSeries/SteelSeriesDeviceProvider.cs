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
    [PluginFeature(Name = "SteelSeries Device Provider")]
    public class SteelSeriesDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x1038;
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public SteelSeriesDeviceProvider(IRgbService rgbService, ILogger logger) : base(RGB.NET.Devices.SteelSeries.SteelSeriesDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;

            RGB.NET.Devices.SteelSeries.SteelSeriesDeviceProvider.DeviceDefinitions.Add(
                0x1726, RGBDeviceType.Mouse, "Rival 650", RGB.NET.Devices.SteelSeries.LedMappings.MouseEightZone, SteelSeriesDeviceType.EightZone
            );
            RGB.NET.Devices.SteelSeries.SteelSeriesDeviceProvider.DeviceDefinitions.Add(
                0x150D, RGBDeviceType.Mousepad, "QCK Prism Cloth", LedMappings.MousepadTwoZone, SteelSeriesDeviceType.TwoZone
            );
        }

        public override void Enable()
        {
            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }

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