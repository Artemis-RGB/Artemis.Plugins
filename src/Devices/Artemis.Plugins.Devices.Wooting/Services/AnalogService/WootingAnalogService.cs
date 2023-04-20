using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Artemis.Core;
using Artemis.Core.Services;
using Serilog;
using WootingAnalogSDKNET;

namespace Artemis.Plugins.Devices.Wooting.Services.AnalogService;

public sealed class WootingAnalogService : IPluginService, IDisposable
{
    private readonly ILogger _logger;
    private readonly List<WootingAnalogDevice> _devices;
    private DateTime _lastUpdate;
    public IReadOnlyCollection<WootingAnalogDevice> Devices { get; }

    public WootingAnalogService(ILogger logger)
    {
        _logger = logger;
                
        (_, WootingAnalogResult initResult) = WootingAnalogSDK.Initialise();

        if (initResult < 0)
            throw new ArtemisPluginException($"Failed to initialise WootingAnalog SDK: {initResult}");

        (List<DeviceInfo> infos, WootingAnalogResult deviceInfoResult) = WootingAnalogSDK.GetConnectedDevicesInfo();
        if (deviceInfoResult < 0)//any value 0 and above means success.
            throw new ArtemisPluginException($"Failed to Get device info from WootingAnalog SDK: {deviceInfoResult}");

        _devices = new(infos.Count);
        foreach (DeviceInfo t in infos)
            _devices.Add(new WootingAnalogDevice(t));

        WootingAnalogSDK.SetKeycodeMode(KeycodeType.HID);

        Devices = new ReadOnlyCollection<WootingAnalogDevice>(_devices);

        ReadAllValues();
    }

    public void Update()
    {
        DateTime now = DateTime.Now;
        if (now - _lastUpdate < TimeSpan.FromSeconds(1.0 / 30.0))
            return;
        
        foreach (WootingAnalogDevice device in _devices)
        {
            (List<(short, float)> data, WootingAnalogResult res) = WootingAnalogSDK.ReadFullBuffer(deviceID: device.Info.device_id);
            if (res != WootingAnalogResult.Ok)
                continue;

            foreach ((short key, float value) in data)
            {
                if (WootingAnalogLedMapping.HidCodesReversed.TryGetValue(key, out RGB.NET.Core.LedId ledId))
                    device.AnalogValues[ledId] = value;
                else
                    _logger.Verbose("Failed to find mapping for hid code {hidCode}", key);
            }
        }
        
        _lastUpdate = now;
    }

    /// <summary>
    /// Reads all known keys just so the datamodel isn't empty.
    /// We do this once so the user can see the keys without having
    /// to press them at least once first.
    /// </summary>
    private void ReadAllValues()
    {
        foreach (WootingAnalogDevice device in _devices)
        {
            foreach ((RGB.NET.Core.LedId ledId, ushort virtualShortCode) in WootingAnalogLedMapping.HidCodes)
            {
                (float analogValue, WootingAnalogResult analogReadResult) = WootingAnalogSDK.ReadAnalog(virtualShortCode, deviceID: device.Info.device_id);

                if (analogReadResult == WootingAnalogResult.NoMapping)
                    continue;

                if (analogReadResult != WootingAnalogResult.Ok)
                    throw new InvalidOperationException();

                device.AnalogValues[ledId] = analogValue;
            }
        }
    }

    #region IDisposable
    public void Dispose()
    {
        WootingAnalogSDK.UnInitialise();
    }
    #endregion
}
