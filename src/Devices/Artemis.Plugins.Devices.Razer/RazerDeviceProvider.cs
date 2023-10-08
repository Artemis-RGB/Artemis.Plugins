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
using RGBDeviceProvider = RGB.NET.Devices.Razer.RazerDeviceProvider;

namespace Artemis.Plugins.Devices.Razer
{
    // ReSharper disable once UnusedMember.Global
    public class RazerDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x1532;
        private readonly ILogger _logger;
        private readonly PluginSettings _pluginSettings;

        private readonly IDeviceService _deviceService;
        private readonly PluginSetting<bool> _loadEmulatorDevices;

        public RazerDeviceProvider(IDeviceService deviceService, PluginSettings pluginSettings, ILogger logger)
        {
            _deviceService = deviceService;
            _pluginSettings = pluginSettings;
            _logger = logger;

            _loadEmulatorDevices = _pluginSettings.GetSetting("LoadEmulatorDevices", false);
            _loadEmulatorDevices.SettingChanged += LoadEmulatorDevicesOnSettingChanged;
            
            // Razer layouts are based on a key map that cannot add LEDs not present in the map
            CreateMissingLedsSupported = false;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RgbDeviceProvider.Exception += Provider_OnException;
            RgbDeviceProvider.LoadEmulatorDevices = _loadEmulatorDevices.Value;

            try
            {
                _deviceService.AddDeviceProvider(this);
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
            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
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