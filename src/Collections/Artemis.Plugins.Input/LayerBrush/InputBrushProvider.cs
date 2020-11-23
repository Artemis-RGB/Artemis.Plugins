using Artemis.Core.LayerBrushes;
using Artemis.Plugins.Input.LayerBrush.Keypress;

namespace Artemis.Plugins.Input.LayerBrush
{
    public class InputBrushProvider : LayerBrushProvider
    {
        public override void Enable()
        {
            RegisterLayerBrushDescriptor<KeypressBrush>("Key press", "Provides effects based on keypresses and button presses", "GestureDoubleTap");
        }

        public override void Disable()
        {
        }
    }
}