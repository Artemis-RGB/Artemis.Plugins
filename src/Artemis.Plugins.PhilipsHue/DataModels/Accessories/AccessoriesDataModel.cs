using System.Collections.Generic;
using System.Linq;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Accessories
{
    public class AccessoriesDataModel : DataModel
    {
        public void UpdateContents(PhilipsHueBridge bridge, List<Sensor> sensors)
        {
            // Motion sensors are split into multiple parts but they share most of their unique ID
            foreach (IGrouping<string, Sensor> sensorGroup in sensors.GroupBy(s => string.Join('-', s.UniqueId.Split("-").Take(2))))
                if (sensorGroup.Count() == 1)
                    AddOrUpdateAccessory(bridge, sensorGroup.First());
                else
                    AddOrUpdateMotionSensor(bridge, sensorGroup.ToList());
        }

        private void AddOrUpdateAccessory(PhilipsHueBridge bridge, Sensor accessory)
        {
            string accessoryKey = $"{bridge.BridgeId}-{accessory.Id}";
            AccessoryDataModel accessoryDataModel = DynamicChild<AccessoryDataModel>(accessoryKey);
            if (accessoryDataModel != null)
                accessoryDataModel.HueSensor = accessory;
            else
                switch (accessory.Type)
                {
                    case "ZLLSwitch":
                        accessoryDataModel = AddDynamicChild(new DimmerSwitch(accessory), accessoryKey);
                        break;
                    case "ZGPSwitch":
                        accessoryDataModel = AddDynamicChild(new Tap(accessory), accessoryKey);
                        break;
                }

            accessoryDataModel.DataModelDescription.Name = accessoryDataModel.Name;
        }

        private void AddOrUpdateMotionSensor(PhilipsHueBridge bridge, List<Sensor> sensors)
        {
            Sensor mainSensor = sensors.First(s => s.State.Presence != null);

            string sensorKey = $"{bridge.BridgeId}-{mainSensor.Id}";
            MotionSensorDataModel accessoryDataModel = DynamicChild<MotionSensorDataModel>(sensorKey);
            if (accessoryDataModel != null)
                accessoryDataModel.Update(mainSensor, sensors);
            else
                accessoryDataModel = AddDynamicChild(new MotionSensorDataModel(mainSensor, sensors), sensorKey);

            accessoryDataModel.DataModelDescription.Name = accessoryDataModel.Name;
        }
    }
}