using System.Collections;
using System.Reflection;
using Artemis.Core;
using Artemis.Core.Nodes;
using Serilog;
using SkiaSharp;

namespace Artemis.Plugins.Nodes.General;

public class GeneralNodesProvider : NodeProvider
{
    private readonly ILogger _logger;

    public GeneralNodesProvider(ILogger logger)
    {
        _logger = logger;
    }

    public override void Enable()
    {
        RegisterTypeColor<bool>(new SKColor(0xFFCD3232));
        RegisterTypeColor<string>(new SKColor(0xFFFFD700));
        RegisterTypeColor<Numeric>(new SKColor(0xFF32CD32));
        RegisterTypeColor<float>(new SKColor(0xFFFF7C00));
        RegisterTypeColor<SKColor>(new SKColor(0xFFAD3EED));
        RegisterTypeColor<IList>(new SKColor(0xFFED3E61));
        RegisterTypeColor<Enum>(new SKColor(0xFF1E90FF));
        RegisterTypeColor<ColorGradient>(new SKColor(0xFF00B2A9));

        foreach (Type nodeType in typeof(GeneralNodesProvider).Assembly.GetTypes().Where(t => typeof(INode).IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract && !t.IsInterface))
        {
            try
            {
                if (nodeType.GetCustomAttribute(typeof(NodeAttribute)) != null)
                    RegisterNodeType(nodeType);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to register node type {NodeType}", nodeType.FullName);
            }
        }
    }

    public override void Disable()
    {
    }
}