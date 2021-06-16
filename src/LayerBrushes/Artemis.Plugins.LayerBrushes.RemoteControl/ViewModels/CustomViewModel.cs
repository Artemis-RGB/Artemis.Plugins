using Artemis.UI.Shared.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.ViewModels
{
    public class CustomViewModel : BrushConfigurationViewModel
    {
        public CustomViewModel(RemoteControlBrush layerBrush) : base(layerBrush)
        {
            RemoteControlBrush = layerBrush;
        }

        public RemoteControlBrush RemoteControlBrush { get; }
    }
}