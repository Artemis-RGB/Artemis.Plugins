using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.DataModels.Groups;
using Q42.HueApi;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Sensors
{
    public class SensorDataModel : DataModel
    {
        public SensorDataModel(Sensor sensor, Bridge bridge)
        {
            Sensor = sensor;
            Bridge = bridge;
        }

        [DataModelIgnore]
        public Sensor Sensor { get; }

        [DataModelIgnore]
        public Bridge Bridge { get; }

        public GroupDataModel Group { get; set; }

        public string Name => Sensor.Name;
        public string SensorType => Sensor.Type;
        public SensorCapabilities Capabilities => Sensor.Capabilities;
        public SensorState State => Sensor.State;

        public string ManufacturerName => Sensor.ManufacturerName;
        public string ModelId => Sensor.ModelId;
        public string ProductId => Sensor.ProductId;
    }
}