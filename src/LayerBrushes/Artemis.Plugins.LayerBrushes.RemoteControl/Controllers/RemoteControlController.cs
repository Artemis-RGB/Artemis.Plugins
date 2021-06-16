using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Plugins.LayerBrushes.RemoteControl.Models;
using Artemis.Plugins.LayerBrushes.RemoteControl.Services;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Controllers
{
    public class RemoteControlController : WebApiController
    {
        private readonly RemoteControlService _remoteControlService;

        public RemoteControlController(RemoteControlService remoteControlService)
        {
            _remoteControlService = remoteControlService;
        }

        [Route(HttpVerbs.Get, "/remote-control-brushes")]
        public IEnumerable<RemoteControlBrushModel> GetRemoteControlBrushes()
        {
            return _remoteControlService.Brushes.Select(b => new RemoteControlBrushModel(b, false));
        }

        [Route(HttpVerbs.Get, "/remote-control-brushes/{brushId}")]
        public RemoteControlBrushModel GetRemoteControlBrush(Guid brushId)
        {
            RemoteControlBrush brush = _remoteControlService.Brushes.FirstOrDefault(b => b.Layer.EntityId == brushId);
            if (brush == null)
                throw HttpException.NotFound("Remote control brush not found.");
            return new RemoteControlBrushModel(brush, true);
        }

        [Route(HttpVerbs.Post, "/remote-control-brushes/{brushId}/update-colors")]
        public async Task RemoteControlBrushUpdateColors(Guid brushId)
        {
            RemoteControlBrush brush = _remoteControlService.Brushes.FirstOrDefault(b => b.Layer.EntityId == brushId);
            if (brush == null)
                throw HttpException.NotFound("Remote control brush not found.");

            List<RemoteControlColorModel> colorModels = await HttpContext.GetRequestDataAsync<List<RemoteControlColorModel>>();
            brush.ApplyLedColors(colorModels);
        }
    }
}