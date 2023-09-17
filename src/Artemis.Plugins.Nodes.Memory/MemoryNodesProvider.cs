using System;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Nodes.Memory.Nodes;
using SkiaSharp;

namespace Artemis.Plugins.Nodes.Memory;

public class MemoryNodesProvider : PluginFeature
{
    private readonly INodeService _nodeService;
    private readonly Plugin _plugin;

    public MemoryNodesProvider(INodeService nodeService, Plugin plugin)
    {
        _nodeService = nodeService;
        _plugin = plugin;
    }

    public override void Enable()
    {
        _nodeService.RegisterTypeColor(_plugin, typeof(UIntPtr), new SKColor(123, 105, 133));
        _nodeService.RegisterNodeType(_plugin, typeof(PointerNode));
    }

    public override void Disable()
    {
        throw new System.NotImplementedException();
    }
}