using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.DataModels.Groups;
using Artemis.Plugins.PhilipsHue.DataModels.Lights;

namespace Artemis.Plugins.PhilipsHue.DataModels
{
    public class HueDataModel : DataModel
    {
        public HueDataModel()
        {
            Rooms = new RoomsDataModel();
            Zones = new ZonesDataModel();
            Lights = new LightsDataModel();
        }

        public RoomsDataModel Rooms { get; set; }
        public ZonesDataModel Zones { get; set; }
        public LightsDataModel Lights { get; set; }
    }
}