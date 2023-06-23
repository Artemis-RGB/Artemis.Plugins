using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using RGB.NET.Devices.Wooting.Enum;
using Serilog;

namespace Artemis.Plugins.Devices.Wooting.Services.ProfileService;

public sealed class WootingProfileService : ReusableService
{
    private readonly ILogger _logger;
    private List<WootingProfileDevice> _devices;
    private DateTime _lastUpdate;
    public IEnumerable<WootingProfileDevice> Devices => IsActivated ? _devices : Enumerable.Empty<WootingProfileDevice>();

    public WootingProfileService(ILogger logger)
    {
        _logger = logger;
    }

    public void Update()
    {
        if (!IsActivated)
            return;
        
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

    protected override void Activate()
    {
        _devices = new List<WootingProfileDevice>();

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

    protected override void Deactivate()
    {
        _devices.Clear();
    }
}
