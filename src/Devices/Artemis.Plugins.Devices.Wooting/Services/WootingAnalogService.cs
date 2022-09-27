using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WootingAnalogSDKNET;
using System.Collections.ObjectModel;
using Artemis.Core;
using Artemis.Core.Services;
using System.Collections.Concurrent;
using Artemis.Plugins.Devices.Wooting.Services;
using Serilog;

namespace Artemis.Plugins.Devices.Wooting.Services;

public class WootingAnalogService : IPluginService, IDisposable
{
    private readonly ILogger _logger;
    private readonly List<WootingAnalogDevice> _devices;
    public IReadOnlyCollection<WootingAnalogDevice> Devices { get; }

    public WootingAnalogService(ILogger logger)
    {
        _logger = logger;
        (int count, WootingAnalogResult result) = WootingAnalogSDK.Initialise();

        if (count < 1 || result != WootingAnalogResult.Ok)
            throw new Exception();

        (List<DeviceInfo> infos, WootingAnalogResult result2) = WootingAnalogSDK.GetConnectedDevicesInfo();
        if (result2 != WootingAnalogResult.Ok)
            throw new Exception();

        _devices = new(infos.Count);
        for (int i = 0; i < infos.Count; i++)
            _devices.Add(new WootingAnalogDevice(infos[i]));

        WootingAnalogSDK.SetKeycodeMode(KeycodeType.HID);

        Devices = new ReadOnlyCollection<WootingAnalogDevice>(_devices);

        ReadAllValues();
    }

    public void Update()
    {
        for (int i = 0; i < _devices.Count; i++)
        {
            WootingAnalogDevice device = _devices[i];
            (List<(short, float)> data, WootingAnalogResult res) = WootingAnalogSDK.ReadFullBuffer(deviceID: device.Info.device_id);
            if (res != WootingAnalogResult.Ok)
                continue;

            foreach ((short key, float value) in data)
            {
                if (LedMapping.HidCodesReversed.TryGetValue(key, out RGB.NET.Core.LedId ledId))
                    device.AnalogValues[ledId] = value;
                else
                    _logger.Verbose("Failed to find mapping for hid code {hidCode}", key);
            }
        }
    }

    /// <summary>
    /// Reads all known keys just so the datamodel isn't empty.
    /// We do this once so the user can see the keys without having
    /// to press them at least once first.
    /// </summary>
    private void ReadAllValues()
    {
        for (int i = 0; i < _devices.Count; i++)
        {
            WootingAnalogDevice device = _devices[i];
            foreach ((RGB.NET.Core.LedId ledId, ushort virtualShortCode) in LedMapping.HidCodes)
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
    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                WootingAnalogSDK.UnInitialise();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
