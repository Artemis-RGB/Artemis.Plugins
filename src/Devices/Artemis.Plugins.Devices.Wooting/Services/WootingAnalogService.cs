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

namespace Artemis.Plugins.Devices.Wooting;

public class WootingAnalogService : IPluginService, IDisposable
{
    private readonly ILogger _logger;
    private readonly List<WootingAnalogDevice> _devices;
    public IReadOnlyCollection<WootingAnalogDevice> Devices { get; }

    public WootingAnalogService(ILogger logger)
    {
        _logger = logger;
        var (count, result) = WootingAnalogSDK.Initialise();

        if (count < 1 || result != WootingAnalogResult.Ok)
            throw new Exception();

        var (infos, result2) = WootingAnalogSDK.GetConnectedDevicesInfo();
        if (result2 != WootingAnalogResult.Ok)
            throw new Exception();

        _devices = new(infos.Count);
        for (int i = 0; i < infos.Count; i++)
        {
            _devices.Add(new WootingAnalogDevice(infos[i]));
        }

        WootingAnalogSDK.SetKeycodeMode(KeycodeType.HID);

        Devices = new ReadOnlyCollection<WootingAnalogDevice>(_devices);

        ReadAllValues();
    }

    public void Update()
    {
        for (int i = 0; i < _devices.Count; i++)
        {
            var device = _devices[i];
            var (data, res) = WootingAnalogSDK.ReadFullBuffer(deviceID: device.Info.device_id);
            if (res != WootingAnalogResult.Ok)
                continue;

            foreach (var item in data)
            {
                if (LedMapping.HidCodesReversed.TryGetValue(item.Item1, out var ledId))
                {
                    device.AnalogValues[ledId] = item.Item2;
                }
                else
                {
                    _logger.Verbose("Failed to find mapping for hid code {hidCode}", item.Item1);
                }
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
            var device = _devices[i];
            foreach ((var ledId, var virtualShortCode) in LedMapping.HidCodes)
            {
                var (analogValue, analogReadResult) = WootingAnalogSDK.ReadAnalog(virtualShortCode, deviceID: device.Info.device_id);

                if (analogReadResult == WootingAnalogResult.NoMapping)
                    continue;

                if (analogReadResult != WootingAnalogResult.Ok)
                    throw new InvalidOperationException();

                device.AnalogValues[ledId] = analogValue;
            }
        }
    }

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                WootingAnalogSDK.UnInitialise();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
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
