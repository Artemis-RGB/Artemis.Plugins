using Artemis.Storage.Entities.Profile.Nodes;

namespace Artemis.Plugins.Nodes.General.Nodes.List;

public class ListOperatorEntity
{
    public NodeScriptEntity? Script { get; set; }
    public ListOperator Operator { get; set; }
}