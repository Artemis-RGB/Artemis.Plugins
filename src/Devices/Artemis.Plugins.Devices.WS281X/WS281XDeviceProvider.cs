using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.WS281X.Settings;
using RGB.NET.Devices.WS281X.Arduino;
using RGB.NET.Devices.WS281X.Bitwizard;
using RGB.NET.Devices.WS281X.NodeMCU;
using Serilog;

namespace Artemis.Plugins.Devices.WS281X
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "WS281X Device Provider")]
    public class WS281XDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;
        private readonly PluginSettings _settings;

        public WS281XDeviceProvider(IRgbService rgbService, PluginSettings settings, ILogger logger) : base(RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance)
        {
            _settings = settings;
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting<List<DeviceDefinition>>("DeviceDefinitions");
            if (definitions.Value == null)
                definitions.Value = new List<DeviceDefinition>();

            string[] ports = SerialPort.GetPortNames();
            foreach (DeviceDefinition deviceDefinition in definitions.Value)
            {
                if (deviceDefinition.Type != DeviceDefinitionType.ESP8266 && !ports.Contains(deviceDefinition.Port))
                {
                    _logger.Warning($"Not adding WS281X device named {deviceDefinition.Name} because {deviceDefinition.Port} was not found");
                    continue;
                }

                if (deviceDefinition.Type == DeviceDefinitionType.ESP8266)
                {
                    UdpClient client = new();
                    try
                    {
                        client.Connect(deviceDefinition.Hostname, 1872);
                    }
                    catch (SocketException e)
                    {
                        _logger.Warning(e, $"Not adding UDP based device named {deviceDefinition.Name} on {deviceDefinition.Hostname}:1872");
                        continue;
                    }
                }

                switch (deviceDefinition.Type)
                {
                    case DeviceDefinitionType.Arduino:
                        RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.AddDeviceDefinition(new ArduinoWS281XDeviceDefinition(deviceDefinition.Port));
                        break;
                    case DeviceDefinitionType.Bitwizard:
                        RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.AddDeviceDefinition(new BitwizardWS281XDeviceDefinition(deviceDefinition.Port));
                        break;
                    case DeviceDefinitionType.ESP8266:
                        RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.AddDeviceDefinition(new NodeMCUWS281XDeviceDefinition(deviceDefinition.Hostname));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RGB.NET.Devices.WS281X.WS281XDeviceProvider.Instance.Dispose();
        }
    }
}