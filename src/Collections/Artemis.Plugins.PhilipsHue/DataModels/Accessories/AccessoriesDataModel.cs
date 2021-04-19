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
            AccessoryDataModel accessoryDataModel = GetDynamicChild<AccessoryDataModel>(accessoryKey)?.Value;
            if (accessoryDataModel != null)
                accessoryDataModel.HueSensor = accessory;
            else
                switch (accessory.Type)
                {
                    case "ZLLSwitch":
                        accessoryDataModel = AddDynamicChild(accessoryKey, new DimmerSwitch(accessory)).Value;
                        break;
                    case "ZGPSwitch":
                        accessoryDataModel = AddDynamicChild(accessoryKey, new Tap(accessory)).Value;
                        break;
                }

            accessoryDataModel.DataModelDescription.Name = accessoryDataModel.Name;
        }

        private void AddOrUpdateMotionSensor(PhilipsHueBridge bridge, List<Sensor> sensors)
        {
            Sensor mainSensor = sensors.First(s => s.State.Presence != null);

            string sensorKey = $"{bridge.BridgeId}-{mainSensor.Id}";
            MotionSensorDataModel accessoryDataModel = GetDynamicChild<MotionSensorDataModel>(sensorKey)?.Value;
            if (accessoryDataModel != null)
                accessoryDataModel.Update(mainSensor, sensors);
            else
                accessoryDataModel = AddDynamicChild(sensorKey, new MotionSensorDataModel(mainSensor, sensors)).Value;

            accessoryDataModel.DataModelDescription.Name = accessoryDataModel.Name;
        }
    }
}