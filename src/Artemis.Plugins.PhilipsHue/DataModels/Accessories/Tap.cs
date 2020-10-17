using System;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Accessories
{
    public class Tap : AccessoryDataModel
    {
        public Tap(Sensor hueSensor) : base(hueSensor)
        {
            AccessoryType = AccessoryType.HueTapSwitch;
        }

        public TapButtonStatus ButtonStatus => (TapButtonStatus) (HueSensor.State.ButtonEvent ?? 0);
        public DateTime LastButtonPress => HueSensor.State.Lastupdated ?? DateTime.MinValue;
    }

    public enum TapButtonStatus
    {
        None = 0,
        Button1 = 34,
        Button2 = 16,
        Button3 = 17,
        Button4 = 18
    }
}