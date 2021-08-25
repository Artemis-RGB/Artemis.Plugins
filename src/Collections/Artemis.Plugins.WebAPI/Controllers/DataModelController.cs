using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Json;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class DataModelController : WebApiController
    {
        private readonly IDataModelService _dataModelService;
        private readonly JsonSerializerSettings _serializerSettings;

        public DataModelController(IDataModelService dataModelService)
        {
            _dataModelService = dataModelService;
            _serializerSettings = new JsonSerializerSettings {PreserveReferencesHandling = PreserveReferencesHandling.Objects, ContractResolver = new DataModelResolver()};
        }

        [Route(HttpVerbs.Get, "/data-model")]
        public async Task GetDataModel()
        {
            List<DataModel> dataModel = _dataModelService.GetDataModels();

            // Use a custom ContractResolver that respects [DataModelIgnore]
            HttpContext.Response.ContentType = MimeType.Json;
            await using TextWriter writer = HttpContext.OpenResponseText();
            string json = JsonConvert.SerializeObject(dataModel, _serializerSettings);
            await writer.WriteAsync(json);
        }

        [Route(HttpVerbs.Get, "/data-model/{plugin}")]
        public async Task GetDataModel(Guid plugin)
        {
            DataModel dataModel = _dataModelService.GetDataModels().FirstOrDefault(dm => dm.Module.Plugin.Guid == plugin);
            if (dataModel == null)
                throw HttpException.NotFound();

            // Use a custom ContractResolver that respects [DataModelIgnore]
            HttpContext.Response.ContentType = MimeType.Json;
            await using TextWriter writer = HttpContext.OpenResponseText();
            string json = JsonConvert.SerializeObject(dataModel, _serializerSettings);
            await writer.WriteAsync(json);
        }
    }
}