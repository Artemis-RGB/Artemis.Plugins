using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Debug.Settings;
using Serilog;

namespace Artemis.Plugins.Devices.Debug
{
    // ReSharper disable once UnusedMember.Global
    public class DebugDeviceProvider : DeviceProvider
    {
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
            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
            if (definitions.Value == null)
                definitions.Value = new List<DeviceDefinition>();

            // foreach (DeviceDefinition deviceDefinition in definitions.Value)
            // RGB.NET.Devices.Debug.DebugDeviceProvider.Instance.AddFakeDeviceDefinition(deviceDefinition.Layout, deviceDefinition.ImageLayout);

            try
            {
                _rgbService.AddDeviceProvider(RgbDeviceProvider);
            }
            catch (Exception e)
            {
                _logger.Warning(e, "Debug device provided failed to initialize, check paths");
            }
        }
    }
}