using System.Linq;
using System.Windows.Navigation;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.ViewModels
{
    public class CustomViewModel : BrushConfigurationViewModel
    {
        public CustomViewModel(RemoteControlBrush layerBrush, IWebServerService webServerService) : base(layerBrush)
        {
            RemoteControlBrush = layerBrush;

            BrushUrl = $"{webServerService.Server!.Listener.Prefixes.First().Replace("*", "localhost")}remote-control-brushes/{RemoteControlBrush.Layer.EntityId}";
        }

        public string BrushUrl { get; }
        public RemoteControlBrush RemoteControlBrush { get; }

        public void OpenHyperlink(object sender, RequestNavigateEventArgs e)
        {
            Utilities.OpenUrl(e.Uri.AbsoluteUri);
        }
    }
}