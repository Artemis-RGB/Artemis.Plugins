using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace Artemis.Plugins.PhilipsHue.DataModels.Groups
{
    public abstract class GroupsDataModel : DataModel
    {
        protected GroupsDataModel()
        {
            Groups = new List<GroupDataModel>();
        }

        [DataModelIgnore]
        public List<GroupDataModel> Groups { get; set; }

        protected void UpdateGroups(PhilipsHueBridge bridge, List<Group> groups)
        {
            foreach (Group group in groups)
            {
                string groupKey = $"{bridge.BridgeId}-{group.Id}";
                GroupDataModel groupDataModel = DynamicChild<GroupDataModel>(groupKey);
                if (groupDataModel != null)
                    groupDataModel.HueGroup = group;
                else
                {
                    groupDataModel = (GroupDataModel) AddDynamicChild(new GroupDataModel(group, bridge.BridgeInfo), groupKey);
                    Groups.Add(groupDataModel);
                }

                groupDataModel.DataModelDescription.Name = groupDataModel.Name;
            }

            // Remove groups that no longer exist
            List<GroupDataModel> groupsToRemove = Groups
                .Where(dmg => dmg.HueBridge.Config.BridgeId == bridge.BridgeId && groups.All(g => g.Id == dmg.HueGroup.Id))
                .ToList();

            foreach (GroupDataModel groupDataModel in groupsToRemove)
            {
                RemoveDynamicChild(groupDataModel);
                Groups.Remove(groupDataModel);
            }
        }

        public void UpdateContents(PhilipsHueBridge bridge, List<Light> lights)
        {
            foreach (Light light in lights)
            {
                List<GroupDataModel> groups = Groups
                    .Where(g => g.HueGroup.Lights.Any(l => l.Equals(light.Id)) && g.HueBridge.Config.BridgeId.Equals(bridge.BridgeId, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                GroupDataModel roomGroup = groups.FirstOrDefault(g => g.GroupType == GroupType.Room);
                foreach (GroupDataModel groupDataModel in groups)
                    groupDataModel.AddOrUpdateLight(light, roomGroup);
            }
        }
    }
}