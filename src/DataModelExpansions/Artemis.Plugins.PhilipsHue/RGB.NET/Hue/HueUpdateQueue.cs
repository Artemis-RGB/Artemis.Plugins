using System.Collections.Generic;
using System.Linq;
using Q42.HueApi.Streaming.Models;
using RGB.NET.Core;
using RGBColor = Q42.HueApi.ColorConverters.RGBColor;

namespace Artemis.Plugins.PhilipsHue.RGB.NET.Hue
{
    public class HueUpdateQueue : UpdateQueue
    {
        private readonly StreamingLight _light;

        public HueUpdateQueue(IDeviceUpdateTrigger updateTrigger, string lightId, StreamingGroup group) : base(updateTrigger)
        {
            _light = group.First(l => l.Id == byte.Parse(lightId));
        }

        protected override void Update(Dictionary<object, Color> dataSet)
        {
            Color color = dataSet.Values.First();

            _light.State.SetBrightness(1);
            _light.State.SetRGBColor(new RGBColor(color.R, color.G, color.B));
        }
    }
}