using Artemis.Core.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Wooting.Services;

public class WootingProfileService : IPluginService
{
    private readonly ILogger _logger;
    private readonly List<WootingProfileDevice> _devices;
    public IReadOnlyCollection<WootingProfileDevice> Devices { get; }

    public WootingProfileService(ILogger logger)
    {
        _logger = logger;
        _devices = new();

        if (!WootingSdk.IsConnected())
        {
            _logger.Error("Wooting SDK is not connected");
            return;
        }

        var keyboardCount = WootingSdk.GetKeyboardCount();
        for (byte i = 0; i < keyboardCount; i++)
        {
            WootingSdk.SelectDevice(i);
            var info = WootingSdk.GetDeviceInfo();
            //HACK: the model names do not match up for each sdk. this makes them match
            info.Model = info.Model.Replace(" ", "");
            _devices.Add(new WootingProfileDevice(info));
        }

        Devices = new ReadOnlyCollection<WootingProfileDevice>(_devices);
    }

    public void Update()
    {
        for (byte i = 0; i < _devices.Count; i++)
        {
            var device = _devices[i];
            WootingSdk.SelectDevice(i);
            device.Profile = WootingSdk.GetProfile(device.Info.V2Interface);
        }
    }
}
