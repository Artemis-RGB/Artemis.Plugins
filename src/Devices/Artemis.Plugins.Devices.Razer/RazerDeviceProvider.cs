using System;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Devices.Razer;

namespace Artemis.Plugins.Devices.Razer
{
    // ReSharper disable once UnusedMember.Global
    public class RazerDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;
        private readonly PluginSettings _pluginSettings;
        private PluginSetting<bool> _loadEmulatorDevices;

        public RazerDeviceProvider(IRgbService rgbService, PluginSettings pluginSettings) : base(RGB.NET.Devices.Razer.RazerDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _pluginSettings = pluginSettings;

            _loadEmulatorDevices = _pluginSettings.GetSetting("LoadEmulatorDevices", false);
            _loadEmulatorDevices.SettingChanged += LoadEmulatorDevicesOnSettingChanged;
        }

        private void LoadEmulatorDevicesOnSettingChanged(object? sender, EventArgs e)
        {
            if (IsEnabled)
            {
                Task.Run(async () =>
                {
                    Disable();
                    await Task.Delay(200);
                    Enable();
                });
            }
        }

        public override void Enable()
        {
            RGB.NET.Devices.Razer.RazerDeviceProvider.Instance.LoadEmulatorDevices = _loadEmulatorDevices.Value;

            try
            {
                _rgbService.AddDeviceProvider(RgbDeviceProvider);
            }
            catch (RazerException e)
            {
                throw new ArtemisPluginException("Failed to activate Razer plugin, error code: " + e.ErrorCode, e);
            }
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RGB.NET.Devices.Razer.RazerDeviceProvider.Instance.Dispose();
        }
    }
}