using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Json;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Basics;
using GenHTTP.Modules.Conversion.Serializers.Json;
using GenHTTP.Modules.IO.Strings;
using GenHTTP.Modules.Webservices;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class DataModelController
    {
        private readonly IDataModelService _dataModelService;
        private readonly JsonSerializerOptions _serializerSettings;

        public DataModelController(IDataModelService dataModelService)
        {
            _dataModelService = dataModelService;
            _serializerSettings = new JsonSerializerOptions(CoreJson.GetJsonSerializerOptions())
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                TypeInfoResolver = new DataModelJsonTypeInfoResolver()
            };
        }

        [ResourceMethod]
        public IResponseBuilder GetDataModel(IRequest request)
        {
            // Cast to object to avoid the generic type being serialized
            // Serialize manually, we want that juicy TypeInfoResolver
            string json = JsonSerializer.Serialize(_dataModelService.GetDataModels().Cast<object>(), _serializerSettings);
            return request.Respond()
                .Status(ResponseStatus.Ok)
                .Content(new StringContent(json))
                .Type(ContentType.ApplicationJson);
        }

        [ResourceMethod(RequestMethod.Get, ":plugin")]
        public IResponseBuilder GetDataModel(IRequest request, Guid plugin)
        {
            object? dataModel = _dataModelService.GetDataModels().FirstOrDefault(dm => dm.Module.Plugin.Guid == plugin);
            if (dataModel == null)
                return request.Respond().Status(ResponseStatus.NotFound);

            // Serialize manually, we want that juicy TypeInfoResolver
            string json = JsonSerializer.Serialize(dataModel, _serializerSettings);
            return request.Respond()
                .Status(ResponseStatus.Ok)
                .Content(new StringContent(json))
                .Type(ContentType.ApplicationJson);
            
        }
    }
}