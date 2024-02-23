using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Serilog;
using WootingAnalogSDKNET;

namespace Artemis.Plugins.Devices.Wooting.Services.AnalogService;

public sealed class WootingAnalogService : ReusableService
{
    private readonly ILogger _logger;
    private List<WootingAnalogDevice> _devices;
    private DateTime _lastUpdate;
    public IEnumerable<WootingAnalogDevice> Devices => _devices;

    public WootingAnalogService(ILogger logger)
    {
        _logger = logger;
        _devices = [];
    }

    public void Update()
    {
        if (!IsActivated)
            return;
        
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
    
    protected override void Activate()
    {
        (_, WootingAnalogResult initResult) = WootingAnalogSDK.Initialise();

        if (initResult == WootingAnalogResult.DLLNotFound)
        {
            //expected when the user doesn't have the SDK installed.
            //probably just log warning and fail gracefully.
            _logger.Warning("Wooting Analog SDK not found. To use analog features, please install it.");
            return;            
        }

        if (initResult < 0)
            throw new ArtemisPluginException($"Failed to initialise WootingAnalog SDK: {initResult}");

        (List<DeviceInfo> infos, WootingAnalogResult deviceInfoResult) = WootingAnalogSDK.GetConnectedDevicesInfo();
        if (deviceInfoResult < 0)//any value 0 and above means success.
            throw new ArtemisPluginException($"Failed to Get device info from WootingAnalog SDK: {deviceInfoResult}");

        foreach (DeviceInfo t in infos)
            _devices.Add(new WootingAnalogDevice(t));

        WootingAnalogSDK.SetKeycodeMode(KeycodeType.HID);

        ReadAllValues();
    }

    protected override void Deactivate()
    {
        WootingAnalogSDK.UnInitialise();
        _devices = null;
    }
}
