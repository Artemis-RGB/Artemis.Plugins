using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Services;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Wooting.Services.AnalogService;

internal static class WootingAnalogLedMapping
{
    internal static Dictionary<LedId, ushort> HidCodes { get; } = new()
    {
        [LedId.Keyboard_A] = 4,
        [LedId.Keyboard_B] = 5,
        [LedId.Keyboard_C] = 6,
        [LedId.Keyboard_D] = 7,
        [LedId.Keyboard_E] = 8,
        [LedId.Keyboard_F] = 9,
        [LedId.Keyboard_G] = 10,
        [LedId.Keyboard_H] = 11,
        [LedId.Keyboard_I] = 12,
        [LedId.Keyboard_J] = 13,
        [LedId.Keyboard_K] = 14,
        [LedId.Keyboard_L] = 15,
        [LedId.Keyboard_M] = 16,
        [LedId.Keyboard_N] = 17,
        [LedId.Keyboard_O] = 18,
        [LedId.Keyboard_P] = 19,
        [LedId.Keyboard_Q] = 20,
        [LedId.Keyboard_R] = 21,
        [LedId.Keyboard_S] = 22,
        [LedId.Keyboard_T] = 23,
        [LedId.Keyboard_U] = 24,
        [LedId.Keyboard_V] = 25,
        [LedId.Keyboard_W] = 26,
        [LedId.Keyboard_X] = 27,
        [LedId.Keyboard_Y] = 28,
        [LedId.Keyboard_Z] = 29,

        [LedId.Keyboard_1] = 30,
        [LedId.Keyboard_2] = 31,
        [LedId.Keyboard_3] = 32,
        [LedId.Keyboard_4] = 33,
        [LedId.Keyboard_5] = 34,
        [LedId.Keyboard_6] = 35,
        [LedId.Keyboard_7] = 36,
        [LedId.Keyboard_8] = 37,
        [LedId.Keyboard_9] = 38,
        [LedId.Keyboard_0] = 39,

        [LedId.Keyboard_Enter] = 40,
        [LedId.Keyboard_Escape] = 41,
        [LedId.Keyboard_Backspace] = 42,
        [LedId.Keyboard_Tab] = 43,
        [LedId.Keyboard_Space] = 44,

        [LedId.Keyboard_MinusAndUnderscore] = 45,
        [LedId.Keyboard_EqualsAndPlus] = 46,
        [LedId.Keyboard_BracketLeft] = 47,
        [LedId.Keyboard_BracketRight] = 48,
        [LedId.Keyboard_Backslash] = 49,
        [LedId.Keyboard_NonUsTilde] = 50,
        [LedId.Keyboard_SemicolonAndColon] = 51,
        [LedId.Keyboard_ApostropheAndDoubleQuote] = 52,
        [LedId.Keyboard_GraveAccentAndTilde] = 53,
        [LedId.Keyboard_CommaAndLessThan] = 54,
        [LedId.Keyboard_PeriodAndBiggerThan] = 55,
        [LedId.Keyboard_SlashAndQuestionMark] = 56,

        [LedId.Keyboard_CapsLock] = 57,

        [LedId.Keyboard_F1] = 58,
        [LedId.Keyboard_F2] = 59,
        [LedId.Keyboard_F3] = 60,
        [LedId.Keyboard_F4] = 61,
        [LedId.Keyboard_F5] = 62,
        [LedId.Keyboard_F6] = 63,
        [LedId.Keyboard_F7] = 64,
        [LedId.Keyboard_F8] = 65,
        [LedId.Keyboard_F9] = 66,
        [LedId.Keyboard_F10] = 67,
        [LedId.Keyboard_F11] = 68,
        [LedId.Keyboard_F12] = 69,

        [LedId.Keyboard_PrintScreen] = 70,
        [LedId.Keyboard_ScrollLock] = 71,
        [LedId.Keyboard_PauseBreak] = 72,
        [LedId.Keyboard_Insert] = 73,
        [LedId.Keyboard_Home] = 74,
        [LedId.Keyboard_PageUp] = 75,
        [LedId.Keyboard_Delete] = 76,
        [LedId.Keyboard_End] = 77,
        [LedId.Keyboard_PageDown] = 78,

        [LedId.Keyboard_ArrowRight] = 79,
        [LedId.Keyboard_ArrowLeft] = 80,
        [LedId.Keyboard_ArrowDown] = 81,
        [LedId.Keyboard_ArrowUp] = 82,

        [LedId.Keyboard_NumLock] = 83,
        [LedId.Keyboard_NumSlash] = 84,
        [LedId.Keyboard_NumAsterisk] = 85,
        [LedId.Keyboard_NumMinus] = 86,
        [LedId.Keyboard_NumPlus] = 87,
        [LedId.Keyboard_NumEnter] = 88,
        [LedId.Keyboard_Num1] = 89,
        [LedId.Keyboard_Num2] = 90,
        [LedId.Keyboard_Num3] = 91,
        [LedId.Keyboard_Num4] = 92,
        [LedId.Keyboard_Num5] = 93,
        [LedId.Keyboard_Num6] = 94,
        [LedId.Keyboard_Num7] = 95,
        [LedId.Keyboard_Num8] = 96,
        [LedId.Keyboard_Num9] = 97,
        [LedId.Keyboard_Num0] = 98,
        [LedId.Keyboard_NumPeriodAndDelete] = 99,

        [LedId.Keyboard_NonUsBackslash] = 100,
        [LedId.Keyboard_Application] = 101,

        [LedId.Keyboard_LeftCtrl] = 224,
        [LedId.Keyboard_LeftShift] = 225,
        [LedId.Keyboard_LeftAlt] = 226,
        [LedId.Keyboard_LeftGui] = 227,
        [LedId.Keyboard_RightCtrl] = 228,
        [LedId.Keyboard_RightShift] = 229,
        [LedId.Keyboard_RightAlt] = 230,
        [LedId.Keyboard_RightGui] = 231,

        /*
         Thanks Simon for handing me this list
         RgbBrightnessUp = 0x401
         RgbBrightnessDown = 0x402
         SelectAnalogProfile1 = 0x403
         SelectAnalogProfile2 = 0x404
         SelectAnalogProfile3 = 0x405
         ModeKey = 0x408
         FnKey = 0x409
         DisableKey = 0x40A
         FnLayerLock = 0x40B
         FnKey2 = 0x40C=
        */
        [LedId.Keyboard_Custom1] = 1027,
        [LedId.Keyboard_Custom2] = 1028,
        [LedId.Keyboard_Custom3] = 1029,
        [LedId.Keyboard_Custom4] = 1032,
        [LedId.Keyboard_Function] = 1033
    };

    internal static Dictionary<short, LedId> HidCodesReversed { get; } = HidCodes.ToDictionary(x => (short)x.Value, x => x.Key);

    internal static Dictionary<KeyboardKey, short> InputKeyCodes { get; } = new()
    {
        [KeyboardKey.A] = 0x04,
        [KeyboardKey.B] = 0x05, //US_B
        [KeyboardKey.C] = 0x06, //US_C
        [KeyboardKey.D] = 0x07, //US_D
        [KeyboardKey.E] = 0x08, //US_E
        [KeyboardKey.F] = 0x09, //US_F
        [KeyboardKey.G] = 0x0a, //US_G
        [KeyboardKey.H] = 0x0b, //US_H
        [KeyboardKey.I] = 0x0c, //US_I
        [KeyboardKey.J] = 0x0d, //US_J
        [KeyboardKey.K] = 0x0e, //US_K
        [KeyboardKey.L] = 0x0f, //US_L
        [KeyboardKey.M] = 0x10, //US_M
        [KeyboardKey.N] = 0x11, //US_N
        [KeyboardKey.O] = 0x12, //US_O
        [KeyboardKey.P] = 0x13, //US_P
        [KeyboardKey.Q] = 0x14, //US_Q
        [KeyboardKey.R] = 0x15, //US_R
        [KeyboardKey.S] = 0x16, //US_S
        [KeyboardKey.T] = 0x17, //US_T
        [KeyboardKey.U] = 0x18, //US_U
        [KeyboardKey.V] = 0x19, //US_V
        [KeyboardKey.W] = 0x1a, //US_W
        [KeyboardKey.X] = 0x1b, //US_X
        [KeyboardKey.Y] = 0x1c, //US_Y
        [KeyboardKey.Z] = 0x1d, //US_Z
        [KeyboardKey.D1] = 0x1e, //DIGIT1
        [KeyboardKey.D2] = 0x1f, //DIGIT2
        [KeyboardKey.D3] = 0x20, //DIGIT3
        [KeyboardKey.D4] = 0x21, //DIGIT4
        [KeyboardKey.D5] = 0x22, //DIGIT5
        [KeyboardKey.D6] = 0x23, //DIGIT6
        [KeyboardKey.D7] = 0x24, //DIGIT7
        [KeyboardKey.D8] = 0x25, //DIGIT8
        [KeyboardKey.D9] = 0x26, //DIGIT9
        [KeyboardKey.D0] = 0x27, //DIGIT0
        [KeyboardKey.Enter] = 0x28, //ENTER
        [KeyboardKey.Escape] = 0x29, //ESCAPE
        [KeyboardKey.Backspace] = 0x2a, //BACKSPACE
        [KeyboardKey.Tab] = 0x2b, //TAB
        [KeyboardKey.Space] = 0x2c, //SPACE
        [KeyboardKey.OemMinus] = 0x2d, //MINUS
        [KeyboardKey.OemPlus] = 0x2e, //EQUAL
        [KeyboardKey.OemOpenBrackets] = 0x2f, //BRACKET_LEFT
        [KeyboardKey.OemCloseBrackets] = 0x30, //BRACKET_RIGHT
        [KeyboardKey.OemBackslash] = 0x31, //BACKSLASH
        [KeyboardKey.OemSemicolon] = 0x33, //SEMICOLON
        [KeyboardKey.OemQuotes] = 0x34, //QUOTE
        [KeyboardKey.OemTilde] = 0x35, //BACKQUOTE
        [KeyboardKey.OemComma] = 0x36, //COMMA
        [KeyboardKey.OemPeriod] = 0x37, //PERIOD
        [KeyboardKey.OemQuestion] = 0x38, //SLASH
        [KeyboardKey.CapsLock] = 0x39, //CAPS_LOCK
        [KeyboardKey.F1] = 0x3a, //F1
        [KeyboardKey.F2] = 0x3b, //F2
        [KeyboardKey.F3] = 0x3c, //F3
        [KeyboardKey.F4] = 0x3d, //F4
        [KeyboardKey.F5] = 0x3e, //F5
        [KeyboardKey.F6] = 0x3f, //F6
        [KeyboardKey.F7] = 0x40, //F7
        [KeyboardKey.F8] = 0x41, //F8
        [KeyboardKey.F9] = 0x42, //F9
        [KeyboardKey.F10] = 0x43, //F10
        [KeyboardKey.F11] = 0x44, //F11
        [KeyboardKey.F12] = 0x45, //F12
        [KeyboardKey.PrintScreen] = 0x46, //PRINT_SCREEN
        [KeyboardKey.ScrollLock] = 0x47, //SCROLL_LOCK
        [KeyboardKey.PauseBreak] = 0x48, //PAUSE
        [KeyboardKey.Insert] = 0x49, //INSERT
        [KeyboardKey.Home] = 0x4a, //HOME
        [KeyboardKey.PageUp] = 0x4b, //PAGE_UP
        [KeyboardKey.Delete] = 0x4c, //DEL
        [KeyboardKey.End] = 0x4d, //END
        [KeyboardKey.PageDown] = 0x4e, //PAGE_DOWN
        [KeyboardKey.ArrowRight] = 0x4f, //ARROW_RIGHT
        [KeyboardKey.ArrowLeft] = 0x50, //ARROW_LEFT
        [KeyboardKey.ArrowDown] = 0x51, //ARROW_DOWN
        [KeyboardKey.ArrowUp] = 0x52, //ARROW_UP
        [KeyboardKey.NumLock] = 0x53, //NUM_LOCK
        [KeyboardKey.NumPadDivide] = 0x54, //NUMPAD_DIVIDE
        [KeyboardKey.NumPadMultiply] = 0x55, //NUMPAD_MULTIPLY
        [KeyboardKey.NumPadSubtract] = 0x56, //NUMPAD_SUBTRACT
        [KeyboardKey.NumPadAdd] = 0x57, //NUMPAD_ADD
        [KeyboardKey.NumPadEnter] = 0x58, //NUMPAD_ENTER
        [KeyboardKey.NumPad1] = 0x59, //NUMPAD1
        [KeyboardKey.NumPad2] = 0x5a, //NUMPAD2
        [KeyboardKey.NumPad3] = 0x5b, //NUMPAD3
        [KeyboardKey.NumPad4] = 0x5c, //NUMPAD4
        [KeyboardKey.NumPad5] = 0x5d, //NUMPAD5
        [KeyboardKey.NumPad6] = 0x5e, //NUMPAD6
        [KeyboardKey.NumPad7] = 0x5f, //NUMPAD7
        [KeyboardKey.NumPad8] = 0x60, //NUMPAD8
        [KeyboardKey.NumPad9] = 0x61, //NUMPAD9
        [KeyboardKey.NumPad0] = 0x62, //NUMPAD0
        [KeyboardKey.NumPadDecimal] = 0x63, //NUMPAD_DECIMAL
        [KeyboardKey.OemPipe] = 0x64, //INTL_BACKSLASH
        [KeyboardKey.Application] = 0x65, //CONTEXT_MENU
        [KeyboardKey.F13] = 0x68, //F13
        [KeyboardKey.F14] = 0x69, //F14
        [KeyboardKey.F15] = 0x6a, //F15
        [KeyboardKey.F16] = 0x6b, //F16
        [KeyboardKey.F17] = 0x6c, //F17
        [KeyboardKey.F18] = 0x6d, //F18
        [KeyboardKey.F19] = 0x6e, //F19
        [KeyboardKey.F20] = 0x6f, //F20
        [KeyboardKey.F21] = 0x70, //F21
        [KeyboardKey.F22] = 0x71, //F22
        [KeyboardKey.F23] = 0x72, //F23
        [KeyboardKey.F24] = 0x73, //F24
        [KeyboardKey.LeftCtrl] = 0xe0, //CONTROL_LEFT
        [KeyboardKey.LeftShift] = 0xe1, //SHIFT_LEFT
        [KeyboardKey.LeftAlt] = 0xe2, //ALT_LEFT
        [KeyboardKey.LeftWin] = 0xe3, //META_LEFT
        [KeyboardKey.RightCtrl] = 0xe4, //CONTROL_RIGHT
        [KeyboardKey.RightShift] = 0xe5, //SHIFT_RIGHT
        [KeyboardKey.RightAlt] = 0xe6, //ALT_RIGHT
        [KeyboardKey.RightWin] = 0xe7, //META_RIGHT
        
        //Mode = 1032,
    };
    
    internal static Dictionary<short, KeyboardKey> InputKeyCodesReversed { get; } = InputKeyCodes.ToDictionary(x => x.Value, x => x.Key);
}