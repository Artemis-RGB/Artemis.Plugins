using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.DataModelExpansions;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Accessories
{
    public class MotionSensorDataModel : AccessoryDataModel
    {
        private Sensor _motionSensor;
        private Sensor _lightSensor;
        private Sensor _temperatureSensor;

        public MotionSensorDataModel(Sensor hueSensor, List<Sensor> sensors) : base(hueSensor)
        {
            _motionSensor = hueSensor;
            _lightSensor = sensors.First(s => s.State.LightLevel != null);
            _temperatureSensor = sensors.First(s => s.State.Temperature != null);

            AccessoryType = AccessoryType.HueMotionSensor;
        }

        public bool IsMotionDetected => _motionSensor.State.Presence ?? false;
        public DateTime LastMotionDetection => _motionSensor.State.Lastupdated ?? DateTime.MinValue;

        [DataModelProperty(Description = "Amount of light measured in lux", Affix = "lx")]
        public double LightLevel => Math.Round(Math.Pow(10, ((_lightSensor.State.LightLevel ?? 0) - 1) / 10000d), 2, MidpointRounding.AwayFromZero);

        [DataModelProperty(Description = "Light level is at or below given dark threshold")]
        public bool Dark => _lightSensor.State.Dark ?? false;

        [DataModelProperty(Description = "Light is at or above light threshold (dark+offset)")]
        public bool Daylight => _lightSensor.State.Daylight ?? false;

        [DataModelProperty(Affix = "°F")]
        public double TemperatureFahrenheit => Math.Round(TemperatureCelsius * 1.8 + 32, 1, MidpointRounding.AwayFromZero);

        [DataModelProperty(Affix = "°C")]
        public double TemperatureCelsius => Math.Round((_temperatureSensor.State.Temperature ?? 0) / 100d, 1, MidpointRounding.AwayFromZero);

        public void Update(Sensor hueSensor, List<Sensor> sensors)
        {
            _motionSensor = hueSensor;
            _lightSensor = sensors.First(s => s.State.LightLevel != null);
            _temperatureSensor = sensors.First(s => s.State.Temperature != null);
        }
    }
}