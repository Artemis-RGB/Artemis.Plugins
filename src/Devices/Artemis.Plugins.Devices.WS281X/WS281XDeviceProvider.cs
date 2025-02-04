﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.WS281X.Settings;
using RGB.NET.Core;
using RGB.NET.Devices.WS281X.Arduino;
using RGB.NET.Devices.WS281X.Bitwizard;
using RGB.NET.Devices.WS281X.NodeMCU;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.WS281X.WS281XDeviceProvider;

namespace Artemis.Plugins.Devices.WS281X
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "WS281X Device Provider")]
    public class WS281XDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private readonly IRenderService _renderService;
        private readonly PluginSettings _settings;

        public WS281XDeviceProvider(ILogger logger, IDeviceService deviceService, IRenderService renderService, PluginSettings settings)
        {
            _logger = logger;
            _deviceService = deviceService;
            _renderService = renderService;
            _settings = settings;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());

            RgbDeviceProvider.Exception += Provider_OnException;
            RgbDeviceProvider.DeviceDefinitions.Clear();
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
                        RgbDeviceProvider.AddDeviceDefinition(new ArduinoWS281XDeviceDefinition(deviceDefinition.Port, deviceDefinition.Speed));
                        break;
                    case DeviceDefinitionType.Bitwizard:
                        RgbDeviceProvider.AddDeviceDefinition(new BitwizardWS281XDeviceDefinition(deviceDefinition.Port, deviceDefinition.Speed));
                        break;
                    case DeviceDefinitionType.ESP8266:
                        RgbDeviceProvider.AddDeviceDefinition(new NodeMCUWS281XDeviceDefinition(deviceDefinition.Hostname));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _deviceService.AddDeviceProvider(this);
        }

        public override void Disable()
        {
            if (_settings.GetSetting("TurnOffLedsOnShutdown", false).Value)
                TurnOffLeds();

            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "WS281X Exception: {message}", args.Exception.Message);

        private void TurnOffLeds()
        {
            // Disable the LEDs on every device before we leave
            foreach (IRGBDevice rgbDevice in RgbDeviceProvider.Devices)
            {
                ListLedGroup _ = new(_renderService.Surface, rgbDevice)
                {
                    Brush = new SolidColorBrush(new Color(0, 0, 0)),
                    ZIndex = 999
                };
            }

            // Don't wait for the next update, force one now and flush all LEDs for good measure
            _renderService.Surface.Update(true);
            // Give the update queues time to process
            Thread.Sleep(200);

            _deviceService.RemoveDeviceProvider(this);
        }
    }
}