using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class PluginsController : WebApiController
    {
        private readonly IPluginManagementService _pluginManagementService;
        private readonly IWebServerService _webServerService;

        public PluginsController(IPluginManagementService pluginManagementService, IWebServerService webServerService)
        {
            _pluginManagementService = pluginManagementService;
            _webServerService = webServerService;
        }

        [Route(HttpVerbs.Get, "/plugins/endpoints")]
        public IEnumerable<PluginEndPoint> GetPluginEndPoints()
        {
            return _webServerService.PluginsModule.PluginEndPoints;
        }

        [Route(HttpVerbs.Get, "/plugins/endpoints/{plugin}/{endPoint}")]
        public PluginEndPoint GetPluginEndPoint(Guid plugin, string endPoint)
        {
            PluginEndPoint pluginEndPoint = _webServerService.PluginsModule.PluginEndPoints.FirstOrDefault(e => e.PluginFeature.Plugin.Guid == plugin && e.Name == endPoint);
            if (pluginEndPoint == null)
                throw HttpException.NotFound();

            return pluginEndPoint;
        }

        [Route(HttpVerbs.Get, "/plugins")]
        public IEnumerable<PluginInfo> GetPlugins()
        {
            return _pluginManagementService.GetAllPlugins().Select(p => p.Info);
        }

        [Route(HttpVerbs.Get, "/plugins/{plugin}")]
        public PluginInfo GetPlugin(Guid plugin)
        {
            PluginInfo pluginInfo = _pluginManagementService.GetAllPlugins().FirstOrDefault(p => p.Guid == plugin)?.Info;
            if (pluginInfo == null)
                throw HttpException.NotFound();

            return pluginInfo;
        }
    }
}