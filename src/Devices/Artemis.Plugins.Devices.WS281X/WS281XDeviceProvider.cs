using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.Plugins.Devices.WS281X.ViewModels;
using RGB.NET.Devices.WS281X.Arduino;
using RGB.NET.Devices.WS281X.Bitwizard;
using Serilog;

namespace Artemis.Plugins.Devices.WS281X
{
    // ReSharper disable once UnusedMember.Global
    public class WS281XDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;
        private readonly PluginSettings _settings;
        private readonly ILogger _logger;

        public WS281XDeviceProvider(IRgbService rgbService, PluginSettings settings, ILogger logger) : base(RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance)
        {
            _settings = settings;
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void EnablePlugin()
        {
            ConfigurationDialog = new PluginConfigurationDialog<WS281XConfigurationViewModel>();

            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting<List<DeviceDefinition>>("DeviceDefinitions");
            if (definitions.Value == null)
                definitions.Value = new List<DeviceDefinition>();

            string[] ports = SerialPort.GetPortNames();
            foreach (DeviceDefinition deviceDefinition in definitions.Value)
            {
                if (!ports.Contains(deviceDefinition.Port))
                {
                    _logger.Warning($"Not adding WS281X device named {deviceDefinition.Name} because {deviceDefinition.Port} was not found");
                    continue;
                }

                switch (deviceDefinition.Type)
                {
                    case DeviceDefinitionType.Arduino:
                        RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.AddDeviceDefinition(new ArduinoWS281XDeviceDefinition(deviceDefinition.Port));
                        break;
                    case DeviceDefinitionType.Bitwizard:
                        RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.AddDeviceDefinition(new BitwizardWS281XDeviceDefinition(deviceDefinition.Port));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void DisablePlugin()
        {
            // TODO: Remove the device provider from the surface
        }
    }
}