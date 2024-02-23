using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.RemoteControl.Models;
using Artemis.Plugins.LayerBrushes.RemoteControl.Services;
using Artemis.Plugins.LayerBrushes.RemoteControl.ViewModels;
using Artemis.UI.Shared.LayerBrushes;
using RGB.NET.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.RemoteControl
{
    // This is the layer brush, the plugin feature has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class RemoteControlBrush : PerLedLayerBrush<EmptyLayerPropertyGroup>
    {
        private readonly RemoteControlService _remoteControlService;

        public RemoteControlBrush(RemoteControlService remoteControlService)
        {
            _remoteControlService = remoteControlService;
        }

        public Dictionary<ArtemisLed, RemoteControlColorModel> LedColors { get; private set; } = new();

        public override void EnableLayerBrush()
        {
            ConfigurationDialog = new LayerBrushConfigurationDialog<CustomViewModel>();
            _remoteControlService.Brushes.Add(this);

            Layer.RenderPropertiesUpdated += LayerOnRenderPropertiesUpdated;
            CreateLedColors();
        }

        public override void DisableLayerBrush()
        {
            Layer.RenderPropertiesUpdated -= LayerOnRenderPropertiesUpdated;

            _remoteControlService.Brushes.Remove(this);
        }

        public override void Update(double deltaTime)
        {
        }

        public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
        {
            return LedColors.TryGetValue(led, out RemoteControlColorModel color) ? color.SKColor : SKColor.Empty;
        }

        public void ApplyLedColors(List<RemoteControlColorModel> colorModels)
        {
            Dictionary<string, RemoteControlColorModel> lookup = LedColors.Values.ToDictionary(lc => lc.LedId, lc => lc);
            foreach (RemoteControlColorModel remoteControlColorModel in colorModels)
            {
                if (!lookup.TryGetValue(remoteControlColorModel.LedId, out RemoteControlColorModel match))
                    continue;

                match.Color = remoteControlColorModel.Color;
                match.SKColor = SKColor.Parse(remoteControlColorModel.Color);
            }
        }

        private void LayerOnRenderPropertiesUpdated(object sender, EventArgs e)
        {
            CreateLedColors();
        }

        private void CreateLedColors()
        {
            Dictionary<ArtemisLed, RemoteControlColorModel> ledColors = new();

            foreach (IGrouping<LedId, ArtemisLed> artemisLeds in Layer.Leds.Distinct().OrderBy(l => l.RgbLed.Id).GroupBy(l => l.RgbLed.Id))
                if (artemisLeds.Count() == 1)
                {
                    string ledId = artemisLeds.Key.ToString();
                    SKColor color = LedColors.FirstOrDefault(c => c.Value.LedId == ledId).Value?.SKColor ?? SKColor.Empty;
                    ledColors.Add(artemisLeds.First(), new RemoteControlColorModel
                    {
                        ArtemisLed = artemisLeds.First(),
                        SKColor = color,
                        LedId = artemisLeds.Key.ToString(),
                        Color = color.ToString()
                    });
                }
                else
                {
                    int index = 1;
                    foreach (ArtemisLed artemisLed in artemisLeds)
                    {
                        string ledId = $"{artemisLeds.Key}.{index}";
                        SKColor color = LedColors.FirstOrDefault(c => c.Value.LedId == ledId).Value?.SKColor ?? SKColor.Empty;
                        ledColors.Add(artemisLed, new RemoteControlColorModel
                        {
                            ArtemisLed = artemisLed,
                            SKColor = color,
                            LedId = $"{artemisLeds.Key}.{index}",
                            Color = color.ToString()
                        });
                        index++;
                    }
                }

            LedColors = ledColors;
        }
    }

    public class EmptyLayerPropertyGroup : LayerPropertyGroup
    {
        #region Overrides of LayerPropertyGroup

        /// <inheritdoc />
        protected override void PopulateDefaults()
        {
        }

        /// <inheritdoc />
        protected override void EnableProperties()
        {
        }

        /// <inheritdoc />
        protected override void DisableProperties()
        {
        }

        #endregion
    }
}