using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Debug.Settings;
using RGB.NET.Core;
using RGB.NET.Devices.Debug;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Debug.DebugDeviceProvider;

namespace Artemis.Plugins.Devices.Debug
{
    // ReSharper disable once UnusedMember.Global
    public class DebugDeviceProvider : DeviceProvider
    {
        private readonly List<ArtemisLayout> _deviceLayouts = new();
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;
        private readonly PluginSettings _settings;

        public DebugDeviceProvider(IRgbService rgbService, PluginSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
            _rgbService = rgbService;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            PopulateDevices();
        }

        public override void Disable()
        {
            // Turn off the LEDs on every device before we leave
            if (_settings.GetSetting("TurnOffLedsOnDisable", true).Value)
            {
                foreach (IRGBDevice rgbDevice in RgbDeviceProvider.Devices)
                {
                    ListLedGroup _ = new(_rgbService.Surface, rgbDevice)
                    {
                        Brush = new SolidColorBrush(new Color(0, 0, 0)),
                        ZIndex = 999
                    };
                }
                // Don't wait for the next update, force one now and flush all LEDs for good measure
                _rgbService.Surface.Update(true);

                // TODO: Remove this when device providers flush on dispose
                Thread.Sleep(200);
                
                // Force devices to update straight away
                foreach (IRGBDevice rgbDevice in RgbDeviceProvider.Devices)
                    rgbDevice.Update(true);
            }

            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }

        public void PopulateDevices()
        {
            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
            if (definitions.Value == null)
                definitions.Value = new List<DeviceDefinition>();

            _deviceLayouts.Clear();
            RgbDeviceProvider.ClearFakeDeviceDefinitions();
            foreach (DeviceDefinition definition in definitions.Value)
            {
                ArtemisLayout layout = new(definition.Layout, LayoutSource.Plugin);
                _deviceLayouts.Add(layout);
                RgbDeviceProvider.AddFakeDeviceDefinition(layout.RgbLayout);
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