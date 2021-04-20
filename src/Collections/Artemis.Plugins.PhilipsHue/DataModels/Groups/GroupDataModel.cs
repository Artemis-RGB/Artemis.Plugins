using System.Runtime.InteropServices;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.DataModels.Lights;
using Q42.HueApi;
using Q42.HueApi.Models;
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
        public GroupType GroupType => HueGroup.Type.Value;
        public RoomClass GroupClass => HueGroup.Class.Value;

        public LightDataModel AddOrUpdateLight(Light light, GroupDataModel roomGroup)
        {
            string lightKey = $"light-{light.Id}";
            LightDataModel lightDataModel = TryGetDynamicChild(lightKey, out DynamicChild<LightDataModel> dynamicChild)
                ? dynamicChild.Value
                : AddDynamicChild(lightKey, new LightDataModel(light), light.Name).Value;

            lightDataModel.HueLight = light;
            lightDataModel.UpdateColor();

            if (GroupType != GroupType.Room && roomGroup != null)
                lightDataModel.DataModelDescription.Name = $"{light.Name} - {roomGroup.Name}";
            return lightDataModel;
        }
    }
}