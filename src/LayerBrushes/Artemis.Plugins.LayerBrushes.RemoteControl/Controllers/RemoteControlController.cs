using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Plugins.LayerBrushes.RemoteControl.Models;
using Artemis.Plugins.LayerBrushes.RemoteControl.Services;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Reflection;
using GenHTTP.Modules.Webservices;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Controllers
{
    public class RemoteControlController
    {
        private readonly RemoteControlService _remoteControlService;

        public RemoteControlController(RemoteControlService remoteControlService)
        {
            _remoteControlService = remoteControlService;
        }

        [ResourceMethod(RequestMethod.Get)]
        public IEnumerable<RemoteControlBrushModel> GetRemoteControlBrushes()
        {
            return _remoteControlService.Brushes.Select(b => new RemoteControlBrushModel(b, false));
        }
        
        [ResourceMethod(RequestMethod.Get, ":brushId")]
        public Result<RemoteControlBrushModel> GetRemoteControlBrush(Guid brushId)
        {
            RemoteControlBrush brush = _remoteControlService.Brushes.FirstOrDefault(b => b.Layer.EntityId == brushId);
            if (brush == null)
                return new Result<RemoteControlBrushModel>(null).Status(ResponseStatus.NotFound);

            return new Result<RemoteControlBrushModel>(new RemoteControlBrushModel(brush, true));
        }

        [ResourceMethod(RequestMethod.Post, ":brushId/update-colors")]
        public IResponseBuilder RemoteControlBrushUpdateColors(IRequest request, Guid brushId, List<RemoteControlColorModel> colorModels)
        {
            RemoteControlBrush brush = _remoteControlService.Brushes.FirstOrDefault(b => b.Layer.EntityId == brushId);
            if (brush == null)
                return request.Respond().Status(ResponseStatus.NotFound);

            brush.ApplyLedColors(colorModels);
            return request.Respond().Status(ResponseStatus.NoContent);
        }
    }
}