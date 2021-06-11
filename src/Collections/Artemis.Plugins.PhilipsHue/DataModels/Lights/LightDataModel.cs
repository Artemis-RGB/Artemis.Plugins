using System;
using Artemis.Core.Modules;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using SkiaSharp;

namespace Artemis.Plugins.PhilipsHue.DataModels.Lights
{
    public class LightDataModel : DataModel
    {
        public LightDataModel(Light hueLight)
        {
            HueLight = hueLight;
        }

        [DataModelIgnore]
        public Light HueLight { get; set; }

        public string Name => HueLight.Name;
        public bool IsOn => HueLight.State.On;

        [DataModelProperty(Description = "The current color of the light")]
        public SKColor SolidColor { get; set; }

        [DataModelProperty(Description = "The current color of the light with the brightness applied as alpha value")]
        public SKColor Color { get; set; }

        [DataModelProperty(Description = "Brightness on a scale of 1 to 255")]
        public byte Brightness => (byte) (HueLight.State.Brightness + 1);

        [DataModelProperty(Description = "A fixed name describing the type of light e.g. “Extended color light”")]
        public string LightType => HueLight.Type;
        
        [DataModelProperty(Name = "Light ID", Description = "A unique identifier for this light within the bridge")]
        public string LightId => HueLight.Id;

        public void UpdateColor()
        {
            RGBColor rgb = HueLight.ToRGBColor();
            SolidColor = new SKColor(ClampToByte(rgb.R * 255), ClampToByte(rgb.G * 255), ClampToByte(rgb.B * 255));
            Color = SolidColor.WithAlpha(Brightness);
        }

        private static byte ClampToByte(double value)
        {
            return (byte) Math.Clamp(value, 0, 255);
        }
    }
}