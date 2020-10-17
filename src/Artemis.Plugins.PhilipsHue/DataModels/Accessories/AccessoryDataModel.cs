using Artemis.Core.DataModelExpansions;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Accessories
{
    public class AccessoryDataModel : DataModel
    {
        public AccessoryDataModel(Sensor hueSensor)
        {
            HueSensor = hueSensor;
        }

        [DataModelIgnore] 
        public Sensor HueSensor { get; set; }

        public string Name => HueSensor.Name;

        public AccessoryType AccessoryType { get; set; }

        // TODO: Remove
        [DataModelProperty(Name = "(PH) State")]
        public SensorState State => HueSensor.State;

        [DataModelProperty(Name = "Sensor ID", Description = "A unique identifier for this sensor within the bridge")]
        public string SensorId => HueSensor.Id;
    }

    public enum AccessoryType
    {
        Other,
        HueDimmerSwitch,
        HueMotionSensor,
        // HueOutdoorSensor,
        // HueSmartButton,
        // HueSmartPlug,
        HueTapSwitch
    }
}