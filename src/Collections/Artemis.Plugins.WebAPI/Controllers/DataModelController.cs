using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Json;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class DataModelController : WebApiController
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

        [Route(HttpVerbs.Get, "/data-model")]
        public async Task GetDataModel()
        {
            // Cast to object to avoid the generic type being serialized
            string json = JsonSerializer.Serialize(_dataModelService.GetDataModels().Cast<object>(), _serializerSettings);

            HttpContext.Response.ContentType = MimeType.Json;
            await using TextWriter writer = HttpContext.OpenResponseText();
            await writer.WriteAsync(json);
        }

        [Route(HttpVerbs.Get, "/data-model/{plugin}")]
        public async Task GetDataModel(Guid plugin)
        {
            // Cast to object to avoid the generic type being serialized
            object dataModel = _dataModelService.GetDataModels().FirstOrDefault(dm => dm.Module.Plugin.Guid == plugin);
            if (dataModel == null)
                throw HttpException.NotFound();
            
            string json = JsonSerializer.Serialize(dataModel, _serializerSettings);

            HttpContext.Response.ContentType = MimeType.Json;
            await using TextWriter writer = HttpContext.OpenResponseText();
            await writer.WriteAsync(json);
        }
    }
}