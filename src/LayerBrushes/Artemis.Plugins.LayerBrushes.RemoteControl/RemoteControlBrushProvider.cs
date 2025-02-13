using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Core.Services;
using Artemis.Plugins.LayerBrushes.RemoteControl.Controllers;

namespace Artemis.Plugins.LayerBrushes.RemoteControl
{
    // This is your plugin feature, it provides Artemis with one or more layer effects via descriptors.
    // Your feature gets enabled once. Your layer effects get enabled multiple times, once for each profile element (folder/layer) it is applied to.
    [PluginFeature]
    public class RemoteControlBrushProvider : LayerBrushProvider
    {
        private readonly IWebServerService _webServerService;

        public RemoteControlBrushProvider(IWebServerService webServerService)
        {
            _webServerService = webServerService;
        }

        public override void Enable()
        {
            _webServerService.AddController<RemoteControlController>(this, "remote-control-brushes");

            // This is where we can register our effect for use, we can also register multiple effects if we'd like
            RegisterLayerBrushDescriptor<RemoteControlBrush>("Remote Control", "A brush that can be remotely controlled via a web API", "Remote");
        }

        public override void Disable()
        {
            // Any registrations we made will be removed automatically, we don't need to do anything here
        }
    }
}