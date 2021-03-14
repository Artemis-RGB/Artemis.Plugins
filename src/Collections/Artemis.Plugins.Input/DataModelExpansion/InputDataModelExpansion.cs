using Artemis.Core.DataModelExpansions;
using Artemis.Core.Services;
using Artemis.Plugins.Input.DataModelExpansion.DataModels;
using Serilog;
using System;

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
            _inputService.KeyboardToggleStatusChanged += InputServiceOnKeyboardToggleStatusChanged;

            DataModel.Keyboard.IsNumLockEnabled = _inputService.KeyboardToggleStatus.NumLock;
            DataModel.Keyboard.IsCapsLockEnabled = _inputService.KeyboardToggleStatus.CapsLock;
            DataModel.Keyboard.IsScrollLockEnabled = _inputService.KeyboardToggleStatus.ScrollLock;
        }

        public override void Disable()
        {
            _inputService.KeyboardKeyUpDown -= InputServiceOnKeyboardKeyUpDown;
            _inputService.MouseButtonUpDown -= InputServiceOnMouseButtonUpDown;
            _inputService.MouseMove -= InputServiceOnMouseMove;
            _inputService.KeyboardToggleStatusChanged -= InputServiceOnKeyboardToggleStatusChanged;
        }

        public override void Update(double deltaTime)
        {
            DataModel.TimeSinceLastInput += TimeSpan.FromSeconds(deltaTime);
        }

        #region Event handlers

        private void InputServiceOnKeyboardKeyUpDown(object sender, ArtemisKeyboardKeyUpDownEventArgs e)
        {
            DataModel.TimeSinceLastInput = TimeSpan.Zero;
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

            if (e.IsDown)
                DataModel.Keyboard.KeyDown.Trigger(new KeyboardEventArgs(e.Key, e.Led?.Device.RgbDevice.DeviceInfo.DeviceName));
            else
                DataModel.Keyboard.KeyUp.Trigger(new KeyboardEventArgs(e.Key, e.Led?.Device.RgbDevice.DeviceInfo.DeviceName));
        }

        private void InputServiceOnMouseButtonUpDown(object sender, ArtemisMouseButtonUpDownEventArgs e)
        {
            DataModel.TimeSinceLastInput = TimeSpan.Zero;
            if (e.IsDown)
            {
                if (!DataModel.Mouse.PressedButtons.Contains(e.Button))
                    DataModel.Mouse.PressedButtons.Add(e.Button);
            }
            else
            {
                DataModel.Mouse.PressedButtons.RemoveAll(k => k == e.Button);
            }

            if (e.IsDown)
                DataModel.Mouse.ButtonDown.Trigger(new MouseEventArgs(e.Button, e.Led?.Device.RgbDevice.DeviceInfo.DeviceName));
            else                
                DataModel.Mouse.ButtonUp.Trigger(new MouseEventArgs(e.Button, e.Led?.Device.RgbDevice.DeviceInfo.DeviceName));
        }

        private void InputServiceOnMouseMove(object sender, ArtemisMouseMoveEventArgs e)
        {
            DataModel.TimeSinceLastInput = TimeSpan.Zero;
            DataModel.Mouse.PositionX = e.CursorX;
            DataModel.Mouse.PositionY = e.CursorY;
        }

        private void InputServiceOnKeyboardToggleStatusChanged(object sender, ArtemisKeyboardToggleStatusArgs e)
        {
            DataModel.TimeSinceLastInput = TimeSpan.Zero;
            DataModel.Keyboard.IsCapsLockEnabled = e.NewStatus.CapsLock;
            DataModel.Keyboard.IsNumLockEnabled = e.NewStatus.NumLock;
            DataModel.Keyboard.IsScrollLockEnabled = e.NewStatus.ScrollLock;
        }

        #endregion
    }
}