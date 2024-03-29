﻿using Artemis.Core;
using Artemis.Core.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    [PluginFeature(AlwaysEnabled = true)]
    public class NoiseBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<NoiseBrush>("Noise", "A brush of that shows an animated random noise", "ScatterPlot");
        }

        public override void Disable()
        {
        }
    }
}