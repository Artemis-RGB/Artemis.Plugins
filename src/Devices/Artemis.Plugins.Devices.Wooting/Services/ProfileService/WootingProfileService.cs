using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Artemis.Core;
using Artemis.Core.Services;
using RGB.NET.Devices.Wooting.Enum;
using Serilog;

namespace Artemis.Plugins.Devices.Wooting.Services.ProfileService;

public sealed class WootingProfileService : IPluginService
{
    private readonly ILogger _logger;
    private readonly List<WootingProfileDevice> _devices;
    private DateTime _lastUpdate;
    public IReadOnlyCollection<WootingProfileDevice> Devices { get; }

    public WootingProfileService(ILogger logger)
    {
        _logger = logger;
        _devices = new List<WootingProfileDevice>();
        Devices = new ReadOnlyCollection<WootingProfileDevice>(_devices);

        if (!WootingSdk.IsConnected())
        {
            throw new ArtemisPluginException("Wooting SDK is not connected");
        }

        byte keyboardCount = WootingSdk.GetDeviceCount();
        for (byte i = 0; i < keyboardCount; i++)
        {
            WootingUsbMeta info = WootingSdk.GetDeviceInfo(i);
            _devices.Add(new WootingProfileDevice(info));
        }
    }

    public void Update()
    {
        DateTime now = DateTime.Now;
        if (now - _lastUpdate < TimeSpan.FromSeconds(1d / 5d))
            return;
        
        if (!WootingSdk.IsConnected())
        {
            _logger.Error("Wooting SDK is not connected");
            return;
        }
        
        for (byte i = 0; i < _devices.Count; i++)
        {
            WootingProfileDevice device = _devices[i];
            if (WootingSdk.TryGetProfile(i, device.Info.V2Interface, out int p))
                device.Profile = p;
        }
        _lastUpdate = now;
    }
}
