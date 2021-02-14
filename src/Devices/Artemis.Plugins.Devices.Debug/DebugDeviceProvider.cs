using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Debug.Settings;
using RGB.NET.Devices.Debug;
using RGB.NET.Layout;
using Serilog;

namespace Artemis.Plugins.Devices.Debug
{
    // ReSharper disable once UnusedMember.Global
    public class DebugDeviceProvider : DeviceProvider
    {
        private readonly List<ArtemisLayout> _deviceLayouts = new();
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;
        private readonly PluginSettings _settings;

        public DebugDeviceProvider(IRgbService rgbService, PluginSettings settings, ILogger logger) : base(RGB.NET.Devices.Debug.DebugDeviceProvider.Instance)
        {
            _settings = settings;
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            PopulateDevices();

        }

        public void PopulateDevices()
        {
            RGB.NET.Devices.Debug.DebugDeviceProvider debugDeviceProvider = (RGB.NET.Devices.Debug.DebugDeviceProvider) RgbDeviceProvider;
            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
            if (definitions.Value == null)
                definitions.Value = new List<DeviceDefinition>();

            _deviceLayouts.Clear();
            debugDeviceProvider.ClearFakeDeviceDefinitions();
            foreach (DeviceDefinition definition in definitions.Value)
            {
                ArtemisLayout layout = new(definition.Layout);
                _deviceLayouts.Add(layout);
                // imageLayout is not used
                debugDeviceProvider.AddFakeDeviceDefinition(layout.RgbLayout, null!);
            }

            try
            {
                _rgbService.AddDeviceProvider(RgbDeviceProvider);
            }
            catch (Exception e)
            {
                _logger.Warning(e, "Debug device provided failed to initialize, check paths");
            }
        }

        public override ArtemisLayout LoadLayout(ArtemisDevice rgbDevice)
        {
            if (rgbDevice.RgbDevice is DebugRGBDevice debugRgbDevice)
            {
                ArtemisLayout artemisLayout = _deviceLayouts.FirstOrDefault(d => d.RgbLayout == debugRgbDevice.Layout);
                return artemisLayout ?? base.LoadLayout(rgbDevice);
            }

            return base.LoadLayout(rgbDevice);
        }
    }
}