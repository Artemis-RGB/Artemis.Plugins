using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Controllers;

namespace Artemis.Plugins.WebAPI.Features
{
    [PluginFeature(Name = "Profiles Web API", Description = "Offers a web API providing access to profiles and their categories")]
    public class ProfilesWebApi : PluginFeature
    {
        private readonly IWebServerService _webServerService;

        public ProfilesWebApi(IWebServerService webServerService)
        {
            _webServerService = webServerService;
        }

        public override void Enable()
        {
            _webServerService.AddController<ProfilesController>(this);
        }

        public override void Disable()
        {
        }
    }
}