using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using RGB.NET.Core;
using RGB.NET.Devices.Razer;
using Serilog;
using Serilog.Events;

namespace Artemis.Plugins.Devices.Razer
{
    // ReSharper disable once UnusedMember.Global
    public class RazerDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x1532;
        private readonly ILogger _logger;
        private readonly PluginSettings _pluginSettings;

        private readonly IRgbService _rgbService;
        private readonly PluginSetting<bool> _loadEmulatorDevices;

        public RazerDeviceProvider(IRgbService rgbService, PluginSettings pluginSettings, ILogger logger) : base(RGB.NET.Devices.Razer.RazerDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _pluginSettings = pluginSettings;
            _logger = logger;

            _loadEmulatorDevices = _pluginSettings.GetSetting("LoadEmulatorDevices", false);
            _loadEmulatorDevices.SettingChanged += LoadEmulatorDevicesOnSettingChanged;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Razer.RazerDeviceProvider.Instance.Exception += Provider_OnException;
            RGB.NET.Devices.Razer.RazerDeviceProvider.Instance.LoadEmulatorDevices = _loadEmulatorDevices.Value;

            try
            {
                _rgbService.AddDeviceProvider(RgbDeviceProvider);
            }
            catch (RazerException e)
            {
                throw new ArtemisPluginException("Failed to activate Razer plugin, error code: " + e.ErrorCode, e);
            }

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGB.NET.Devices.Razer.RazerDeviceProvider.Instance.Exception -= Provider_OnException;
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Razer Exception: {message}", args.Exception.Message);

        private void LoadEmulatorDevicesOnSettingChanged(object sender, EventArgs e)
        {
            if (IsEnabled)
                Task.Run(async () =>
                {
                    Disable();
                    await Task.Delay(200);
                    Enable();
                });
        }

        private void LogDeviceIds()
        {
            List<HidDevice> devices = DeviceList.Local.GetHidDevices(VENDOR_ID).DistinctBy(d => d.ProductID).ToList();
            _logger.Debug("Found {count} Razer device(s)", devices.Count);
            foreach (HidDevice hidDevice in devices)
            {
                try
                {
                    _logger.Debug("Found Razer device {name} with PID 0x{pid}", hidDevice.GetFriendlyName(), hidDevice.ProductID.ToString("X"));
                }
                catch (Exception)
                {
                    _logger.Debug("Found Razer device with PID 0x{pid}", hidDevice.ProductID.ToString("X"));
                }
            }
        }
    }
}