namespace Artemis.Plugins.Devices.Wooting.Services.ProfileService;

public class WootingProfileDevice
{
    public WootingUsbMeta Info { get; }
    
    public int Profile { get; set; }

    public WootingProfileDevice(WootingUsbMeta meta)
    {
        Info = meta;
    }
}