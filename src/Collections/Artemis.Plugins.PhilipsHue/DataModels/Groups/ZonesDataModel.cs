using System.Collections.Generic;
using System.Linq;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi.Models.Groups;

namespace Artemis.Plugins.PhilipsHue.DataModels.Groups
{
    public class ZonesDataModel : GroupsDataModel
    {
        public void Update(PhilipsHueBridge bridge, List<Group> groups)
        {
            UpdateGroups(bridge, groups.Where(g => g.Type == GroupType.Zone).ToList());
        }
    }
}