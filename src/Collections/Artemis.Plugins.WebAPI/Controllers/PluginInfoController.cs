using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Reflection;
using GenHTTP.Modules.Webservices;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class PluginInfoController
    {
        private readonly IPluginManagementService _pluginManagementService;
        private readonly IWebServerService _webServerService;

        public PluginInfoController(IPluginManagementService pluginManagementService, IWebServerService webServerService)
        {
            _pluginManagementService = pluginManagementService;
            _webServerService = webServerService;
        }

        [ResourceMethod]
        public IEnumerable<PluginInfo> GetPlugins()
        {
            return _pluginManagementService.GetAllPlugins().Select(p => p.Info);
        }

        // Provide a pattern matching GUIDs to avoid conflicts with the GetPluginEndPoints method
        [ResourceMethod(RequestMethod.Get, "(?<plugin>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})")]
        public Result<PluginInfo?> GetPlugin(Guid plugin)
        {
            PluginInfo? pluginInfo = _pluginManagementService.GetAllPlugins().FirstOrDefault(p => p.Guid == plugin)?.Info;
            if (pluginInfo == null)
                return new Result<PluginInfo?>(null).Status(ResponseStatus.NotFound);

            return new Result<PluginInfo?>(pluginInfo);
        }
        
        [ResourceMethod(RequestMethod.Get, "endpoints")]
        public IEnumerable<PluginEndPoint> GetPluginEndPoints()
        {
            return _webServerService.PluginsHandler.PluginEndPoints;
        }

        [ResourceMethod(RequestMethod.Get, "endpoints/:plugin/:endPoint")]
        public Result<PluginEndPoint?> GetPluginEndPoint(Guid plugin, string endPoint)
        {
            PluginEndPoint? pluginEndPoint = _webServerService.PluginsHandler.PluginEndPoints.FirstOrDefault(e => e.PluginFeature.Plugin.Guid == plugin && e.Name == endPoint);
            if (pluginEndPoint == null)
                return new Result<PluginEndPoint?>(null).Status(ResponseStatus.NotFound);
        
            return new Result<PluginEndPoint?>(pluginEndPoint);
        }
    }
}