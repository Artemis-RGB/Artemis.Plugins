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

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            
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
                ArtemisLayout layout = new(definition.Layout, LayoutSource.Plugin);
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