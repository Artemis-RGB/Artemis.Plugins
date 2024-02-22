using Artemis.Core;
using Artemis.Plugins.Nodes.General.Nodes.Static.Screens;

namespace Artemis.Plugins.Nodes.General.Nodes.Static;

[Node("Display Value", "Displays an input value for testing purposes.", "Static", InputType = typeof(object))]
public class DisplayValueNode : Node<string, DisplayValueNodeCustomViewModel>
{
    public DisplayValueNode()
    {
        Input = CreateInputPin<object>();
    }

    public InputPin<object> Input { get; }

    public override void Evaluate()
    {
    }
}