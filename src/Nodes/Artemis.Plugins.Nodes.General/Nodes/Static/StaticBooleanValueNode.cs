﻿using Artemis.Core;
using Artemis.Plugins.Nodes.General.Nodes.Static.Screens;

namespace Artemis.Plugins.Nodes.General.Nodes.Static;

[Node("Boolean-Value", "Outputs a configurable static boolean value.", "Static", OutputType = typeof(bool))]
public class StaticBooleanValueNode : Node<bool, StaticBooleanValueNodeCustomViewModel>
{
    #region Constructors

    public StaticBooleanValueNode()
    {
        Name = "Boolean";
        Output = CreateOutputPin<bool>();
    }

    #endregion

    #region Properties & Fields

    public OutputPin<bool> Output { get; }

    #endregion

    #region Methods

    public override void Evaluate()
    {
        Output.Value = Storage;
    }

    #endregion
}