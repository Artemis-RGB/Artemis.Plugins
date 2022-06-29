using Artemis.Core.LayerBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Wooting.LayerBrushes
{
    public class WootingLayerBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<WootingAnalogLayerBrush>("Wooting Analog Layer", "Analog feedback for Wooting keyboards", "SineWave");
        }

        public override void Disable()
        {
        }
    }
}
