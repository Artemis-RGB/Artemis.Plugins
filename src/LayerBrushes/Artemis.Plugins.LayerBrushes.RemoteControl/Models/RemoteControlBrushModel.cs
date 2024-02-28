using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Models
{
    public class RemoteControlBrushModel
    {
        public RemoteControlBrushModel(RemoteControlBrush brush, bool includeLeds)
        {
            ProfileId = brush.Layer.Profile.EntityId;
            ProfileName = brush.Layer.Profile.Name;
            LayerName = brush.Layer.Name;
            LayerId = brush.Layer.EntityId;

            if (includeLeds)
                LedColors = new List<RemoteControlColorModel>(brush.LedColors.Values);
        }

        public Guid ProfileId { get; set; }
        public string ProfileName { get; set; }
        public Guid LayerId { get; set; }
        public string LayerName { get; set; }

        public List<RemoteControlColorModel> LedColors { get; set; }
    }

    public class RemoteControlColorModel
    {
        [JsonIgnore]
        public ArtemisLed ArtemisLed { get; set; }

        [JsonIgnore]
        public SKColor SKColor { get; set; }

        public string LedId { get; set; }
        public string Color { get; set; }
    }
}