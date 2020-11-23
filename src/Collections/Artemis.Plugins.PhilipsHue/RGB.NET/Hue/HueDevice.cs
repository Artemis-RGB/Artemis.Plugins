using System.Collections.Generic;
using System.Linq;
using RGB.NET.Core;

namespace Artemis.Plugins.PhilipsHue.RGB.NET.Hue
{
    public class HueDevice : AbstractRGBDevice<HueDeviceInfo>
    {
        public HueDevice(HueDeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public override HueDeviceInfo DeviceInfo { get; }

        public void Initialize(HueUpdateQueue updateQueue)
        {
            UpdateQueue = updateQueue;
            InitializeLayout();

            Rectangle ledRectangle = new Rectangle(this.Select(x => x.LedRectangle));
            Size = ledRectangle.Size + new Size(ledRectangle.Location.X, ledRectangle.Location.Y);
        }

        private void InitializeLayout()
        {
            // Models based on https://developers.meethue.com/develop/hue-api/supported-devices/#Supported-lights
            switch (DeviceInfo.Model)
            {
                // Hue bulb A19 (E27)
                case "LCA001":
                case "LCA007":
                case "LCA0010":
                case "LCA0014":
                case "LCA0015":
                case "LCA0016":
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(62)).Shape = Shape.Circle;
                    break;
                // Hue Spot BR30 (quick Google search makes it seem like an older generation)
                case "LCT002":
                case "LCT011":
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(62)).Shape = Shape.Circle;
                    break;
                // Hue Spot GU10
                case "LCT003":
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(50)).Shape = Shape.Circle;
                    break;
                // Hue Go
                case "LLC020":
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(150)).Shape = Shape.Circle;
                    break;
                // Hue LightStrips Plus
                case "LCL001":
                    InitializeLed(LedId.LedStripe1, new Point(0, 0), new Size(2000, 14));
                    break;
                // Hue color candle	
                case "LCT012":
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(39)).Shape = Shape.Circle;
                    break;
                default:
                    InitializeLed(LedId.Custom1, new Point(0, 0), new Size(10));
                    break;
            }
        }

        public HueUpdateQueue UpdateQueue { get; private set; }
        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate) => UpdateQueue.SetData(ledsToUpdate);
    }
}