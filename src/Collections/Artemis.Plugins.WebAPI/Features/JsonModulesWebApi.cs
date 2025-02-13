using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Controllers;

namespace Artemis.Plugins.WebAPI.Features;

[PluginFeature(Name = "JSON Modules Web API", Description = "Offers a web API which can be used to create and update data model modules using JSON")]
public class JsonModulesWebApi : Module<JsonModulesDataModel>
{
    private readonly IWebServerService _webServerService;

    public JsonModulesWebApi(IWebServerService webServerService)
    {
        _webServerService = webServerService;
    }
    public override void Enable()
    {
        _webServerService.AddController<JsonModulesController>(this, "json-modules");
    }

    public override void Disable()
    {
    }

    public override void Update(double deltaTime)
    {
    }

    public override List<IModuleActivationRequirement>? ActivationRequirements { get; } = null;
}

public class JsonModulesDataModel : DataModel
{
}