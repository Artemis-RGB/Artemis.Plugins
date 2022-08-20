using Artemis.Core.Modules;
using Artemis.Plugins.PhilipsHue.DataModels.Accessories;
using Artemis.Plugins.PhilipsHue.DataModels.Groups;

namespace Artemis.Plugins.PhilipsHue.DataModels;

public class HueDataModel : DataModel
{
    public HueDataModel()
    {
        Rooms = new RoomsDataModel();
        Zones = new ZonesDataModel();
        Accessories = new AccessoriesDataModel();
    }

    public RoomsDataModel Rooms { get; set; }
    public ZonesDataModel Zones { get; set; }
    public AccessoriesDataModel Accessories { get; set; }
}