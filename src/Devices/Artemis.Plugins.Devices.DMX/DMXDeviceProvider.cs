using System.Collections.Generic;
using System.Threading;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.DMX.Settings;
using RGB.NET.Core;
using RGB.NET.Devices.DMX.E131;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.DMX.DMXDeviceProvider;

namespace Artemis.Plugins.Devices.DMX
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "DMX Device Provider")]
    public class DMXDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private readonly IRenderService _renderService;
        private readonly PluginSettings _settings;

        public DMXDeviceProvider(ILogger logger, IDeviceService deviceService, IRenderService renderService, PluginSettings settings)
        {
            _logger = logger;
            _deviceService = deviceService;
            _renderService = renderService;
            _settings = settings;
        }

        public override void Disable()
        {
            if (_settings.GetSetting("TurnOffLedsOnShutdown", false).Value)
                TurnOffLeds();

            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RgbDeviceProvider.Exception += Provider_OnException;

            PluginSetting<List<DeviceDefinition>> definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
            RgbDeviceProvider.DeviceDefinitions.Clear();
            foreach (DeviceDefinition deviceDefinition in definitions.Value)
            {
                E131DMXDeviceDefinition definition = new(deviceDefinition.Hostname)
                {
                    Port = deviceDefinition.Port,
                    Manufacturer = deviceDefinition.Manufacturer ?? "Artemis",
                    Model = deviceDefinition.Model ?? "DMX Device",
                    Universe = deviceDefinition.Universe
                };

                for (int i = 0; i < deviceDefinition.LedDefinitions.Count; i++)
                {
                    LedDefinition ledDefinition = deviceDefinition.LedDefinitions[i];
                    LedId ledId = LedId.LedStripe1 + i;
                    int rChannel = ledDefinition.R;
                    int gChannel = ledDefinition.G;
                    int bChannel = ledDefinition.B;
                    definition.AddLed(ledId, (rChannel, c => c.GetR()), (gChannel, c => c.GetG()), (bChannel, c => c.GetB()));
                }

                RgbDeviceProvider.AddDeviceDefinition(definition);
            }

            _deviceService.AddDeviceProvider(this);
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "DMX Exception: {message}", args.Exception.Message);

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