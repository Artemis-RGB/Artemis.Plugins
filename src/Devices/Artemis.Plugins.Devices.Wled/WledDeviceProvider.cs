using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Wled.Settings;
using Artemis.Plugins.Devices.Wled.ViewModels;
using RGB.NET.Core;
using RGB.NET.Devices.WLED;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.WLED.WledDeviceProvider;

namespace Artemis.Plugins.Devices.Wled;

// ReSharper disable once UnusedMember.Global
[PluginFeature(Name = "WLED Device Provider")]
public class WledDeviceProvider(ILogger logger, IDeviceService deviceService, PluginSettings settings)
    : DeviceProvider
{
    #region Properties & Fields

    public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

    #endregion

    #region Methods

    public override void Disable()
    {
        deviceService.RemoveDeviceProvider(this);
        RgbDeviceProvider.Exception -= Provider_OnException;
        RgbDeviceProvider.Dispose();
    }

    public override void Enable()
    {
        RgbDeviceProvider.Exception += Provider_OnException;

        RgbDeviceProvider.DeviceDefinitions.Clear();

        PluginSetting<List<DeviceDefinition>> definitions = settings.GetSetting(nameof(WledConfigurationViewModel.DeviceDefinitions), new List<DeviceDefinition>());

        List<(string hostname, string manufacturer, string model)> devices = definitions.Value
                                                                                        .Select(deviceDefinition => (deviceDefinition.Hostname, deviceDefinition.Manufacturer, deviceDefinition.Model))
                                                                                        .ToList();

        if (settings.GetSetting(nameof(WledConfigurationViewModel.EnableAutoDiscovery), false).Value)
        {
            int autoDiscoveryTime = settings.GetSetting(nameof(WledConfigurationViewModel.AutoDiscoveryTime), 500).Value;
            int autoDiscoveryMaxDevices = settings.GetSetting(nameof(WledConfigurationViewModel.AutoDiscoveryMaxDevices), 0).Value;

            foreach ((string address, WledInfo info) in WledDiscoveryHelper.DiscoverDevices(autoDiscoveryTime, autoDiscoveryMaxDevices))
                if (devices.All(x => x.hostname != address))
                    devices.Add((address, info.Brand, info.Product));
        }

        foreach ((string hostname, string manufacturer, string model) in devices)
            RgbDeviceProvider.AddDeviceDefinition(new WledDeviceDefinition(hostname, manufacturer ?? "Artemis", model ?? "WLED Device"));

        deviceService.AddDeviceProvider(this);
    }

    private void Provider_OnException(object sender, ExceptionEventArgs args) => logger.Debug(args.Exception, "WLED Exception: {message}", args.Exception.Message);

    #endregion
}