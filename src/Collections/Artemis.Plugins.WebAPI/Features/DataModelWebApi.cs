using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Controllers;

namespace Artemis.Plugins.WebAPI.Features
{
    [PluginFeature(Name = "Data Model Web API", Description = "Offers a web API providing access to the data model", Icon = "DatabaseSearch")]
    public class DataModelWebApi : PluginFeature
    {
        private readonly IWebServerService _webServerService;

        public DataModelWebApi(IWebServerService webServerService)
        {
            _webServerService = webServerService;
        }

        public override void Enable()
        {
            _webServerService.AddController<DataModelController>(this);
        }

        public override void Disable()
        {
        }
    }
}