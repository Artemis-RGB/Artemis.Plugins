using System.Collections.Generic;
using Artemis.Core.Services;

namespace Artemis.Plugins.LayerBrushes.RemoteControl.Services
{
    public class RemoteControlService : IPluginService
    {
        public List<RemoteControlBrush> Brushes { get; } = new();
    }
}