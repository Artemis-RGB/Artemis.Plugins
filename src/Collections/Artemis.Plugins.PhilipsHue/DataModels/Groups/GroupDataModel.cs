using Artemis.Core.Modules;
using Artemis.Plugins.PhilipsHue.DataModels.Lights;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;

namespace Artemis.Plugins.PhilipsHue.DataModels.Groups
{
    public class GroupDataModel : DataModel
    {
        public GroupDataModel(Group hueGroup, Bridge hueBridge)
        {
            HueGroup = hueGroup;
            HueBridge = hueBridge;
        }

        [DataModelIgnore]
        public Group HueGroup { get; set; }

        [DataModelIgnore]
        public Bridge HueBridge { get; }

        public string Name => HueGroup.Name;
        public GroupType GroupType => HueGroup.Type ?? GroupType.Room;
        public RoomClass GroupClass => HueGroup.Class ?? RoomClass.Other;

        public LightDataModel AddOrUpdateLight(Light light, GroupDataModel roomGroup)
        {
            // Find or add the dynamic child and update it with the latest light info
            string lightKey = $"light-{light.Id}";
            LightDataModel lightDataModel = TryGetDynamicChild(lightKey, out DynamicChild<LightDataModel> dynamicChild)
                ? dynamicChild.Value
                : AddDynamicChild(lightKey, new LightDataModel(light), light.Name).Value;

            // Simply setting HueLight will cause all properties (which are calculated properties) to update
            lightDataModel.HueLight = light;
            // Colors are likely accessed often and are therefore calculated once per update
            lightDataModel.UpdateColor();

            // If not in a room, add the zone name to the light name
            if (GroupType != GroupType.Room && roomGroup != null)
                lightDataModel.DataModelDescription.Name = $"{light.Name} - {roomGroup.Name}";
            return lightDataModel;
        }
    }
}