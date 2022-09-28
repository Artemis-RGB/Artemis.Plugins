namespace Artemis.Plugins.Devices.Wooting.Services;

public class WootingProfileDevice
{
    public WootingUsbMeta Info { get; }
    
    public int Profile { get; set; }

    public WootingProfileDevice(WootingUsbMeta meta)
    {
        Info = meta;
    }
}