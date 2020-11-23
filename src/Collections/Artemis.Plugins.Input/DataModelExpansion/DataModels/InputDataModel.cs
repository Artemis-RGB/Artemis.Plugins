using System.Collections.Generic;
using Artemis.Core.DataModelExpansions;
using Artemis.Core.Services;

namespace Artemis.Plugins.Input.DataModelExpansion.DataModels
{
    public class InputDataModel : DataModel
    {
        public InputDataModel()
        {
            Keyboard = new KeyboardInputDataModel();
            Mouse = new MouseInputDataModel();
        }

        public KeyboardInputDataModel Keyboard { get; }
        public MouseInputDataModel Mouse { get; }
    }

    public class KeyboardInputDataModel : DataModel
    {
        public KeyboardInputDataModel()
        {
            PressedKeys = new List<KeyboardKey>();
        }

        public bool IsShiftDown { get; set; }
        public bool IsControlDown { get; set; }
        public bool IsAltDown { get; set; }
        public bool IsWindowsDown { get; set; }

        [DataModelProperty(Description = "A list containing all currently pressed keys")]
        public List<KeyboardKey> PressedKeys { get; set; }
    }

    public class MouseInputDataModel : DataModel
    {
        public MouseInputDataModel()
        {
            PressedButtons = new List<MouseButton>();
        }

        [DataModelProperty(Name = "Cursor X position", Description = "The X position of the mouse cursor in pixels")]
        public int PositionX { get; set; }

        [DataModelProperty(Name = "Cursor Y position", Description = "The Y position of the mouse cursor in pixels")]
        public int PositionY { get; set; }

        [DataModelProperty(Description = "A list containing all currently pressed buttons")]
        public List<MouseButton> PressedButtons { get; set; }
    }
}