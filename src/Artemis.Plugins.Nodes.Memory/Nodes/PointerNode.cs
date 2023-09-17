using System;
using Artemis.Core;

namespace Artemis.Plugins.Nodes.Memory.Nodes;

[Node("Memory Pointer", "Gets a pointer at the given address", "Memory", InputType = typeof(string), OutputType = typeof(IntPtr))]
public class PointerNode : Node
{
    public override void Evaluate()
    {
        throw new System.NotImplementedException();
    }
}