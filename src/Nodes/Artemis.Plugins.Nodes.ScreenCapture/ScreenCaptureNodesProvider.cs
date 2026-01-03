using Artemis.Core.Nodes;
using Artemis.Plugins.Nodes.ScreenCapture.Nodes;

namespace Artemis.Plugins.Nodes.ScreenCapture;

public class ScreenCaptureNodesProvider : NodeProvider
{
    public override void Enable()
    {
        RegisterNodeType<PixelCaptureNode>();
    }

    public override void Disable()
    {
    }
}