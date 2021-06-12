using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Devices.OpenRGB;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using OpenRGBClient = OpenRGB.NET.OpenRGBClient;

namespace Artemis.Plugins.Devices.OpenRGB
{
    [PluginFeature(Name = "OpenRGB Device Provider")]
    public class OpenRGBDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        private readonly PluginSetting<List<OpenRGBServerDefinition>> _deviceDefinitionsSettings;
        private readonly PluginSetting<bool> _forceAddAllDevicesSetting;
        private readonly Timer _reconnectTimer;

        public OpenRGBDeviceProvider(IRgbService rgbService, PluginSettings settings, ILogger logger) : base(RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance)
        {
            _logger = logger;
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
            CreateMissingLedsSupported = false;
            RemoveExcessiveLedsSupported = true;

            _reconnectTimer = new Timer(30 * 1000);
            _reconnectTimer.Elapsed += OnReconnectTimerElapsed;
        }

        public override void Enable()
        {
            foreach (OpenRGBServerDefinition def in _deviceDefinitionsSettings.Value)
            {
                RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.DeviceDefinitions.Add(def);
            }

            RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.ForceAddAllDevices = _forceAddAllDevicesSetting.Value;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            bool anyFailedToConnect = false;
            foreach (OpenRGBServerDefinition deviceDefinition in RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.DeviceDefinitions.Where(dd => !dd.Connected))
            {
                _logger.Error("OpenRGB server {ip}:{port} failed to connect: {error}", deviceDefinition.Ip, deviceDefinition.Port, deviceDefinition.LastError);
                anyFailedToConnect = true;
            }

            if (anyFailedToConnect)
            {
                _logger.Information("Failed to connect to at least one OpenRGB server. Retrying in 30secs...");
                _reconnectTimer.Start();
            }
            else
            {
                _reconnectTimer.Stop();
            }
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();
        }

        private async void OnReconnectTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //if all device definitions are connected, stop the timer and return.
            if (RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.DeviceDefinitions.All(dd => dd.Connected))
            {
                _logger.Verbose("OpenRGB reconnect timer elapsed, but all device definitions connected successfully. Stopping timer.");
                _reconnectTimer.Stop();
                return;
            }

            //otherwise, check if we can connect to any of the not-yet-connected device definitions.
            bool restart = false;
            foreach (OpenRGBServerDefinition item in RGB.NET.Devices.OpenRGB.OpenRGBDeviceProvider.Instance.DeviceDefinitions.Where(dd => !dd.Connected))
            {
                try
                {
                    OpenRGBClient dummyClient = new OpenRGBClient(item.Ip, item.Port, "Artemis server test", true);
                    restart |= true;
                    dummyClient.Dispose();
                }
                catch { }
            }

            //if we can connect using the dummy client, restart the plugin.
            if (restart)
            {
                Disable();
                await Task.Delay(200);
                Enable();
            }
        }
    }
}