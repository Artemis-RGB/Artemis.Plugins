using Artemis.Core.Modules;

namespace Artemis.Plugins.Devices.Wooting
{
    public class WootingDataModel : DataModel
    {
        public WootingAnalogDataModel Analog { get; set; } = new();

        public int Profile { get; set; }

        public bool IsInAnalogProfile => Profile != 0;

        public bool IsInDigitalProfile => Profile == 0;
    }

    public class WootingAnalogDataModel : DataModel
    {

    }
}
