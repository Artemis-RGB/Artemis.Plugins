using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Devices.OpenRGB;

namespace Artemis.Plugins.Devices.OpenRGB
{
    [PluginFeature(Name = "OpenRGB Device Provider")]
    public class OpenRGBDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        private readonly PluginSetting<List<OpenRGBServerDefinition>> _deviceDefinitionsSettings;
        private readonly PluginSetting<bool> _forceAddAllDevicesSetting;

        public OpenRGBDeviceProvider(IRgbService rgbService, PluginSettings settings) : base(RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _forceAddAllDevicesSetting = settings.GetSetting("ForceAddAllDevices", false);
            _deviceDefinitionsSettings = settings.GetSetting("DeviceDefinitions", new List<OpenRGBServerDefinition>
            {
                new()
                {
                    ClientName = "Artemis",
                    Ip = "127.0.0.1",
                    Port = 6742
                }
            });
        }

        public override void Enable()
        {
            foreach (OpenRGBServerDefinition def in _deviceDefinitionsSettings.Value)
                RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.DeviceDefinitions.Add(def);

            RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.ForceAddAllDevices = _forceAddAllDevicesSetting.Value;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.Dispose();
        }
    }
}