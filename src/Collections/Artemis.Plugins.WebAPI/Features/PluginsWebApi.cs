using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Controllers;

namespace Artemis.Plugins.WebAPI.Features
{
    [PluginFeature(Name = "Plugins Web API", Description = "Offers a web API providing access to enabled plugins", Icon = "Connection")]
    public class PluginsWebApi : PluginFeature
    {
        private readonly IWebServerService _webServerService;

        public PluginsWebApi(IWebServerService webServerService)
        {
            _webServerService = webServerService;
        }

        public override void Enable()
        {
            _webServerService.AddController<PluginsController>(this);
        }

        public override void Disable()
        {
        }
    }
}