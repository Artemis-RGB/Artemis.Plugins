using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Artemis.Plugins.WebAPI.DataModels;
using Artemis.Plugins.WebAPI.Services;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Reflection;
using GenHTTP.Modules.Webservices;
using Json.Schema;

namespace Artemis.Plugins.WebAPI.Controllers;

public class JsonModulesController(IJsonModuleService jsonModuleService)
{
    [ResourceMethod]
    public IEnumerable<JsonModule> GetJsonModules()
    {
        return jsonModuleService.JsonModules;
    }

    [ResourceMethod(RequestMethod.Post, ":moduleId/schema")]
    public Result<JsonModule?> AddJsonModuleSchema(IRequest request, string moduleId, JsonSchema schema)
    {
        if (moduleId.Length > 50)
            return new Result<JsonModule?>(null).Status(400, "Module ID must be 50 characters or less");
        
        JsonModule jsonModule = new()
        {
            ModuleId = moduleId,
            Schema = schema
        };

        jsonModuleService.AddJsonModule(jsonModule);
        jsonModuleService.SaveChanges();

        return new Result<JsonModule?>(jsonModule);
    }

    [ResourceMethod(RequestMethod.Put, ":moduleId/schema")]
    public IResponseBuilder UpdateJsonModuleSchema(IRequest request, string moduleId, JsonSchema schema)
    {
        JsonModule? jsonModule = jsonModuleService.GetJsonModule(moduleId);
        if (jsonModule == null)
            return request.Respond().Status(ResponseStatus.NotFound);

        jsonModule.Schema = schema;
        jsonModuleService.SaveChanges();

        return request.Respond().Status(ResponseStatus.NoContent);
    }

    [ResourceMethod(RequestMethod.Put, ":moduleId/data")]
    public IResponseBuilder UpdateJsonModuleData(IRequest request, string moduleId, JsonObject data)
    {
        JsonSchemaDataModel? dataModel = jsonModuleService.GetJsonModuleDataModel(moduleId);
        if (dataModel == null)
            return request.Respond().Status(ResponseStatus.NotFound);

        dataModel.ApplyJsonObject(data);

        return request.Respond().Status(ResponseStatus.NoContent);
    }
}