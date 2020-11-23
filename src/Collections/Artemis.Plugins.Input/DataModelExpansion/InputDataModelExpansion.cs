using Artemis.Core.DataModelExpansions;
using Artemis.Core.Services;
using Artemis.Plugins.Input.DataModelExpansion.DataModels;
using Serilog;

namespace Artemis.Plugins.Input.DataModelExpansion
{
    public class InputDataModelExpansion : DataModelExpansion<InputDataModel>
    {
        private readonly IInputService _inputService;

        public InputDataModelExpansion(ILogger logger, IInputService inputService)
        {
            _inputService = inputService;
        }

        public override void Enable()
        {
            _inputService.KeyboardKeyUpDown += InputServiceOnKeyboardKeyUpDown;
            _inputService.MouseButtonUpDown += InputServiceOnMouseButtonUpDown;
            _inputService.MouseMove += InputServiceOnMouseMove;
        }

        public override void Disable()
        {
            _inputService.KeyboardKeyUpDown -= InputServiceOnKeyboardKeyUpDown;
            _inputService.MouseButtonUpDown -= InputServiceOnMouseButtonUpDown;
            _inputService.MouseMove -= InputServiceOnMouseMove;
        }

        public override void Update(double deltaTime)
        {
        }

        #region Event handlers

        private void InputServiceOnKeyboardKeyUpDown(object sender, ArtemisKeyboardKeyUpDownEventArgs e)
        {
            if (e.IsDown)
            {
                if (!DataModel.Keyboard.PressedKeys.Contains(e.Key))
                    DataModel.Keyboard.PressedKeys.Add(e.Key);
            }
            else
            {
                DataModel.Keyboard.PressedKeys.RemoveAll(k => k == e.Key);
            }

            DataModel.Keyboard.IsAltDown = e.Modifiers.HasFlag(KeyboardModifierKey.Alt);
            DataModel.Keyboard.IsControlDown = e.Modifiers.HasFlag(KeyboardModifierKey.Control);
            DataModel.Keyboard.IsShiftDown = e.Modifiers.HasFlag(KeyboardModifierKey.Shift);
            DataModel.Keyboard.IsWindowsDown = e.Modifiers.HasFlag(KeyboardModifierKey.Windows);
        }

        private void InputServiceOnMouseButtonUpDown(object sender, ArtemisMouseButtonUpDownEventArgs e)
        {
            if (e.IsDown)
            {
                if (!DataModel.Mouse.PressedButtons.Contains(e.Button))
                    DataModel.Mouse.PressedButtons.Add(e.Button);
            }
            else
            {
                DataModel.Mouse.PressedButtons.RemoveAll(k => k == e.Button);
            }
        }

        private void InputServiceOnMouseMove(object sender, ArtemisMouseMoveEventArgs e)
        {
            DataModel.Mouse.PositionX = e.CursorX;
            DataModel.Mouse.PositionY = e.CursorY;
        }

        #endregion
    }
}