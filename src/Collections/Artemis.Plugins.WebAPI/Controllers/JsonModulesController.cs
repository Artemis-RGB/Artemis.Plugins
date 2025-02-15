using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Artemis.Plugins.WebAPI.DataModels;
using Artemis.Plugins.WebAPI.Services;
using GenHTTP.Api.Protocol;
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

    [ResourceMethod(RequestMethod.Post)]
    public JsonModule AddJsonModuleSchema(JsonSchema schema)
    {
        JsonModule jsonModule = new()
        {
            ModuleId = Guid.NewGuid(),
            Schema = schema
        };

        jsonModuleService.AddJsonModule(jsonModule);
        jsonModuleService.SaveChanges();

        return jsonModule;
    }

    [ResourceMethod(RequestMethod.Put, ":moduleId/schema")]
    public IResponseBuilder UpdateJsonModuleSchema(IRequest request, Guid moduleId, JsonSchema schema)
    {
        JsonModule? jsonModule = jsonModuleService.GetJsonModule(moduleId);
        if (jsonModule == null)
            return request.Respond().Status(ResponseStatus.NotFound);

        jsonModule.Schema = schema;
        jsonModuleService.SaveChanges();

        return request.Respond().Status(ResponseStatus.NoContent);
    }

    [ResourceMethod(RequestMethod.Put, ":moduleId/data")]
    public IResponseBuilder UpdateJsonModuleData(IRequest request, Guid moduleId, JsonObject data)
    {
        JsonSchemaDataModel? dataModel = jsonModuleService.GetJsonModuleDataModel(moduleId);
        if (dataModel == null)
            return request.Respond().Status(ResponseStatus.NotFound);

        dataModel.ApplyJsonObject(data);

        return request.Respond().Status(ResponseStatus.NoContent);
    }
}