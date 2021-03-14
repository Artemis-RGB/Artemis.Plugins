using System;
using System.Collections.Generic;
using Artemis.Core;
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
        public TimeSpan TimeSinceLastInput { get; set; }
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

        public bool IsCapsLockEnabled { get; set; }
        public bool IsNumLockEnabled { get; set; }
        public bool IsScrollLockEnabled { get; set; }

        [DataModelProperty(Description = "A list containing all currently pressed keys")]
        public List<KeyboardKey> PressedKeys { get; set; }

        [DataModelProperty(Description = "An event that triggers each time a keyboard key is pressed down")]
        public DataModelEvent<KeyboardEventArgs> KeyDown { get; set; } = new();
        [DataModelProperty(Description = "An event that triggers each time a keyboard key is released")]
        public DataModelEvent<KeyboardEventArgs> KeyUp { get; set; } = new();
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

        [DataModelProperty(Description = "An event that triggers each time a mouse button is pressed down")]
        public DataModelEvent<MouseEventArgs> ButtonDown { get; set; } = new();
        [DataModelProperty(Description = "An event that triggers each time a mouse button is released")]
        public DataModelEvent<MouseEventArgs> ButtonUp { get; set; } = new();
    }

    public class KeyboardEventArgs : DataModelEventArgs
    {
        public KeyboardEventArgs(KeyboardKey key, string device)
        {
            Key = key;
            Device = device;
        }

        [DataModelProperty(Description = "The key that was pressed")]
        public KeyboardKey Key { get; set; }
        [DataModelProperty(Description = "The name of the device on which the key was pressed")]
        public string Device { get; set; }
    }

    public class MouseEventArgs : DataModelEventArgs
    {
        public MouseEventArgs(MouseButton button, string device)
        {
            Button = button;
            Device = device;
        }

        [DataModelProperty(Description = "The button that was pressed")]
        public MouseButton Button { get; set; }
        [DataModelProperty(Description = "The name of the device on which the key was pressed")]
        public string Device { get; set; }
    }
}