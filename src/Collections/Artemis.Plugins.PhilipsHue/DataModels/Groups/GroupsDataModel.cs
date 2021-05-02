using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi;
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

        public void UpdateContents(PhilipsHueBridge bridge, List<Light> lights)
        {
            foreach (Light light in lights)
            {
                // Find dynamic children of the same bridge containing the lights we've received
                // TODO: Light IDs might be globally unique, won't need to ensure the bridge matches
                List<GroupDataModel> groups = Groups
                    .Where(g => g.HueBridge.Config != null &&
                                g.HueGroup.Lights.Any(l => l.Equals(light.Id)) &&
                                g.HueBridge.Config.BridgeId.Equals(bridge.BridgeId, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                GroupDataModel roomGroup = groups.FirstOrDefault(g => g.GroupType == GroupType.Room);
                foreach (GroupDataModel groupDataModel in groups)
                    groupDataModel.AddOrUpdateLight(light, roomGroup);
            }
        }

        protected void UpdateGroups(PhilipsHueBridge bridge, List<Group> groups)
        {
            // Add or update missing groups
            foreach (Group group in groups)
            {
                string groupKey = $"{bridge.BridgeId}-{group.Id}";
                
                GroupDataModel groupDataModel = null;
                if (TryGetDynamicChild(groupKey, out DynamicChild<GroupDataModel> dynamicChild)) 
                    groupDataModel = dynamicChild.Value;

                if (groupDataModel != null)
                    groupDataModel.HueGroup = group;
                else
                {
                    groupDataModel = AddDynamicChild(groupKey, new GroupDataModel(group, bridge.BridgeInfo)).Value;
                    Groups.Add(groupDataModel);
                }

                groupDataModel.DataModelDescription.Name = groupDataModel.Name;
            }

            // Remove groups that no longer exist
            List<GroupDataModel> groupsToRemove = Groups
                .Where(dmg => dmg.HueBridge.Config?.BridgeId == bridge.BridgeId && groups.All(g => g.Id == dmg.HueGroup.Id))
                .ToList();

            foreach (GroupDataModel groupDataModel in groupsToRemove)
            {
                RemoveDynamicChildByKey($"{bridge.BridgeId}-{groupDataModel.HueGroup.Id}");
                Groups.Remove(groupDataModel);
            }
        }
    }
}