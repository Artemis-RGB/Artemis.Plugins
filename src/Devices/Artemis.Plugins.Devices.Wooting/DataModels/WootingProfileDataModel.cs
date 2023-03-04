using Artemis.Core.Modules;

namespace Artemis.Plugins.Devices.Wooting.DataModels;

public class WootingProfileDataModel : DataModel
{
    public int Profile { get; set; }

    public bool IsInAnalogProfile => Profile != 0;

    public bool IsInDigitalProfile => Profile == 0;
}