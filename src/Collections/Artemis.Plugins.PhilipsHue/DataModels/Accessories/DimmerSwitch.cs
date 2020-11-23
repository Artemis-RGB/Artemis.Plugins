using System;
using System.ComponentModel;
using Q42.HueApi.Models;

namespace Artemis.Plugins.PhilipsHue.DataModels.Accessories
{
    public class DimmerSwitch : AccessoryDataModel
    {
        public DimmerSwitch(Sensor hueSensor) : base(hueSensor)
        {
            AccessoryType = AccessoryType.HueDimmerSwitch;
        }

        public DimmerButtonStatus ButtonStatus => (DimmerButtonStatus) (HueSensor.State.ButtonEvent ?? 0);
        public DateTime LastButtonPress => HueSensor.State.Lastupdated ?? DateTime.MinValue;
    }

    public enum DimmerButtonStatus
    {
        None = 0,
        [Description("On - Initial press")]
        Button1InitialPress = 1000,
        [Description("On - Hold")]
        Button1Hold = 1001,
        [Description("On - Short released")]
        Button1ShortReleased = 1002,
        [Description("On - Long released")]
        Button1LongReleased = 1003,
        [Description("Dim up - Initial press")]
        Button2InitialPress = 2000,
        [Description("Dim up - Hold")]
        Button2Hold = 2001,
        [Description("Dim up - Short released")]
        Button2ShortReleased = 2002,
        [Description("Dim up - Long released")]
        Button2LongReleased = 2003,
        [Description("Dim down - Initial press")]
        Button3InitialPress = 3000,
        [Description("Dim down - Hold")]
        Button3Hold = 3001,
        [Description("Dim down - Short released")]
        Button3ShortReleased = 3002,
        [Description("Dim down - Long released")]
        Button3LongReleased = 3003,
        [Description("Off - Initial press")]
        Button4InitialPress = 4000,
        [Description("Off - Hold")]
        Button4Hold = 4001,
        [Description("Off - Short released")]
        Button4ShortReleased = 4002,
        [Description("Off - Long released")]
        Button4LongReleased = 4003,
    }
}