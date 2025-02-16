using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Controllers;
using Artemis.Plugins.WebAPI.Services;

namespace Artemis.Plugins.WebAPI.Features;

[PluginFeature(Name = "JSON Modules Web API", Description = "Offers a web API which can be used to create and update data model modules using JSON")]
public class JsonModulesWebApi : Module<JsonModulesDataModel>
{
    private readonly IWebServerService _webServerService;
    private readonly IJsonModuleService _jsonModuleService;

    public JsonModulesWebApi(IWebServerService webServerService, IJsonModuleService jsonModuleService)
    {
        _webServerService = webServerService;
        _jsonModuleService = jsonModuleService;
    }
    public override void Enable()
    {
        _webServerService.AddController<JsonModulesController>(this, "json-modules");
        _jsonModuleService.Load();
    }

    public override void Disable()
    {
        _jsonModuleService.Unload();
    }

    public override DataModelPropertyAttribute GetDataModelDescription()
    {
        return new DataModelPropertyAttribute()
        {
            Name = "JSON Modules",
            Description = "Contains all the data model modules that are created using the JSON Modules Web API"
        };
    }

    public override void Update(double deltaTime)
    {
    }

    public override List<IModuleActivationRequirement>? ActivationRequirements { get; } = null;
}

public class JsonModulesDataModel : DataModel
{
}