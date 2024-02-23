using Artemis.Core;
using Artemis.Plugins.Nodes.General.Nodes.Transition.Screens;

namespace Artemis.Plugins.Nodes.General.Nodes.Transition;

[Node("Easing Function", "Outputs a selectable easing function", "Transition", OutputType = typeof(Easings.Functions))]
public class EasingFunctionNode : Node<Easings.Functions, EasingFunctionNodeCustomViewModel>
{
    public EasingFunctionNode()
    {
        Output = CreateOutputPin<Easings.Functions>();
    }

    public OutputPin<Easings.Functions> Output { get; }

    public override void Evaluate()
    {
        Output.Value = Storage;
    }
}