using Artemis.Core.Modules;

namespace Artemis.Plugins.Devices.Wooting;

public class WootingDeviceDataModel : DataModel
{
    public WootingAnalogDataModel Analog { get; set; } = new();

    public int Profile { get; set; }

    public bool IsInAnalogProfile => Profile != 0;

    public bool IsInDigitalProfile => Profile == 0;
}
