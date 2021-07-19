using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Razer
{
    public static class LedMappings
    {
        public const int KEYBOARD_MAX_COLUMN = 24;

        public static readonly LedMapping<int> KeyboardBlackWidowV3 = new()
        {
            //Row 0 is empty

            #region Row 1

            [LedId.Keyboard_Escape] = (KEYBOARD_MAX_COLUMN * 1) + 2,
            [LedId.Keyboard_F1] = (KEYBOARD_MAX_COLUMN * 1) + 4,
            [LedId.Keyboard_F2] = (KEYBOARD_MAX_COLUMN * 1) + 5,
            [LedId.Keyboard_F3] = (KEYBOARD_MAX_COLUMN * 1) + 6,
            [LedId.Keyboard_F4] = (KEYBOARD_MAX_COLUMN * 1) + 7,
            [LedId.Keyboard_F5] = (KEYBOARD_MAX_COLUMN * 1) + 8,
            [LedId.Keyboard_F6] = (KEYBOARD_MAX_COLUMN * 1) + 9,
            [LedId.Keyboard_F7] = (KEYBOARD_MAX_COLUMN * 1) + 10,
            [LedId.Keyboard_F8] = (KEYBOARD_MAX_COLUMN * 1) + 11,
            [LedId.Keyboard_F9] = (KEYBOARD_MAX_COLUMN * 1) + 12,
            [LedId.Keyboard_F10] = (KEYBOARD_MAX_COLUMN * 1) + 13,
            [LedId.Keyboard_F11] = (KEYBOARD_MAX_COLUMN * 1) + 14,
            [LedId.Keyboard_F12] = (KEYBOARD_MAX_COLUMN * 1) + 15,
            [LedId.Keyboard_PrintScreen] = (KEYBOARD_MAX_COLUMN * 1) + 16,
            [LedId.Keyboard_ScrollLock] = (KEYBOARD_MAX_COLUMN * 1) + 17,
            [LedId.Keyboard_PauseBreak] = (KEYBOARD_MAX_COLUMN * 1) + 18,
            [LedId.Logo] = (KEYBOARD_MAX_COLUMN * 1) + 21,

            #endregion

            #region Row 2

            [LedId.Keyboard_Programmable1] = (KEYBOARD_MAX_COLUMN * 2) + 1,
            [LedId.Keyboard_GraveAccentAndTilde] = (KEYBOARD_MAX_COLUMN * 2) + 2,
            [LedId.Keyboard_1] = (KEYBOARD_MAX_COLUMN * 2) + 3,
            [LedId.Keyboard_2] = (KEYBOARD_MAX_COLUMN * 2) + 4,
            [LedId.Keyboard_3] = (KEYBOARD_MAX_COLUMN * 2) + 5,
            [LedId.Keyboard_4] = (KEYBOARD_MAX_COLUMN * 2) + 6,
            [LedId.Keyboard_5] = (KEYBOARD_MAX_COLUMN * 2) + 7,
            [LedId.Keyboard_6] = (KEYBOARD_MAX_COLUMN * 2) + 8,
            [LedId.Keyboard_7] = (KEYBOARD_MAX_COLUMN * 2) + 9,
            [LedId.Keyboard_8] = (KEYBOARD_MAX_COLUMN * 2) + 10,
            [LedId.Keyboard_9] = (KEYBOARD_MAX_COLUMN * 2) + 11,
            [LedId.Keyboard_0] = (KEYBOARD_MAX_COLUMN * 2) + 12,
            [LedId.Keyboard_MinusAndUnderscore] = (KEYBOARD_MAX_COLUMN * 2) + 13,
            [LedId.Keyboard_EqualsAndPlus] = (KEYBOARD_MAX_COLUMN * 2) + 14,
            [LedId.Keyboard_Backspace] = (KEYBOARD_MAX_COLUMN * 2) + 15,
            [LedId.Keyboard_Insert] = (KEYBOARD_MAX_COLUMN * 2) + 16,
            [LedId.Keyboard_Home] = (KEYBOARD_MAX_COLUMN * 2) + 17,
            [LedId.Keyboard_PageUp] = (KEYBOARD_MAX_COLUMN * 2) + 18,
            [LedId.Keyboard_NumLock] = (KEYBOARD_MAX_COLUMN * 2) + 19,
            [LedId.Keyboard_NumSlash] = (KEYBOARD_MAX_COLUMN * 2) + 20,
            [LedId.Keyboard_NumAsterisk] = (KEYBOARD_MAX_COLUMN * 2) + 21,
            [LedId.Keyboard_NumMinus] = (KEYBOARD_MAX_COLUMN * 2) + 22,

            #endregion

            #region Row 3

            [LedId.Keyboard_Programmable2] = (KEYBOARD_MAX_COLUMN * 3) + 1,
            [LedId.Keyboard_Tab] = (KEYBOARD_MAX_COLUMN * 3) + 2,
            [LedId.Keyboard_Q] = (KEYBOARD_MAX_COLUMN * 3) + 3,
            [LedId.Keyboard_W] = (KEYBOARD_MAX_COLUMN * 3) + 4,
            [LedId.Keyboard_E] = (KEYBOARD_MAX_COLUMN * 3) + 5,
            [LedId.Keyboard_R] = (KEYBOARD_MAX_COLUMN * 3) + 6,
            [LedId.Keyboard_T] = (KEYBOARD_MAX_COLUMN * 3) + 7,
            [LedId.Keyboard_Y] = (KEYBOARD_MAX_COLUMN * 3) + 8,
            [LedId.Keyboard_U] = (KEYBOARD_MAX_COLUMN * 3) + 9,
            [LedId.Keyboard_I] = (KEYBOARD_MAX_COLUMN * 3) + 10,
            [LedId.Keyboard_O] = (KEYBOARD_MAX_COLUMN * 3) + 11,
            [LedId.Keyboard_P] = (KEYBOARD_MAX_COLUMN * 3) + 12,
            [LedId.Keyboard_BracketLeft] = (KEYBOARD_MAX_COLUMN * 3) + 13,
            [LedId.Keyboard_BracketRight] = (KEYBOARD_MAX_COLUMN * 3) + 14,
            [LedId.Keyboard_Backslash] = (KEYBOARD_MAX_COLUMN * 3) + 15,
            [LedId.Keyboard_Delete] = (KEYBOARD_MAX_COLUMN * 3) + 16,
            [LedId.Keyboard_End] = (KEYBOARD_MAX_COLUMN * 3) + 17,
            [LedId.Keyboard_PageDown] = (KEYBOARD_MAX_COLUMN * 3) + 18,
            [LedId.Keyboard_Num7] = (KEYBOARD_MAX_COLUMN * 3) + 19,
            [LedId.Keyboard_Num8] = (KEYBOARD_MAX_COLUMN * 3) + 20,
            [LedId.Keyboard_Num9] = (KEYBOARD_MAX_COLUMN * 3) + 21,
            [LedId.Keyboard_NumPlus] = (KEYBOARD_MAX_COLUMN * 3) + 22,

            #endregion

            #region Row 4

            [LedId.Keyboard_Programmable3] = (KEYBOARD_MAX_COLUMN * 4) + 1,
            [LedId.Keyboard_CapsLock] = (KEYBOARD_MAX_COLUMN * 4) + 2,
            [LedId.Keyboard_A] = (KEYBOARD_MAX_COLUMN * 4) + 3,
            [LedId.Keyboard_S] = (KEYBOARD_MAX_COLUMN * 4) + 4,
            [LedId.Keyboard_D] = (KEYBOARD_MAX_COLUMN * 4) + 5,
            [LedId.Keyboard_F] = (KEYBOARD_MAX_COLUMN * 4) + 6,
            [LedId.Keyboard_G] = (KEYBOARD_MAX_COLUMN * 4) + 7,
            [LedId.Keyboard_H] = (KEYBOARD_MAX_COLUMN * 4) + 8,
            [LedId.Keyboard_J] = (KEYBOARD_MAX_COLUMN * 4) + 9,
            [LedId.Keyboard_K] = (KEYBOARD_MAX_COLUMN * 4) + 10,
            [LedId.Keyboard_L] = (KEYBOARD_MAX_COLUMN * 4) + 11,
            [LedId.Keyboard_SemicolonAndColon] = (KEYBOARD_MAX_COLUMN * 4) + 12,
            [LedId.Keyboard_ApostropheAndDoubleQuote] = (KEYBOARD_MAX_COLUMN * 4) + 13,
            [LedId.Keyboard_NonUsTilde] = (KEYBOARD_MAX_COLUMN * 4) + 14,
            [LedId.Keyboard_Enter] = (KEYBOARD_MAX_COLUMN * 4) + 15,
            [LedId.Keyboard_Num4] = (KEYBOARD_MAX_COLUMN * 4) + 19,
            [LedId.Keyboard_Num5] = (KEYBOARD_MAX_COLUMN * 4) + 20,
            [LedId.Keyboard_Num6] = (KEYBOARD_MAX_COLUMN * 4) + 21,

            #endregion

            #region Row 5

            [LedId.Keyboard_Programmable4] = (KEYBOARD_MAX_COLUMN * 5) + 1,
            [LedId.Keyboard_LeftShift] = (KEYBOARD_MAX_COLUMN * 5) + 2,
            [LedId.Keyboard_NonUsBackslash] = (KEYBOARD_MAX_COLUMN * 5) + 3,
            [LedId.Keyboard_Z] = (KEYBOARD_MAX_COLUMN * 5) + 4,
            [LedId.Keyboard_X] = (KEYBOARD_MAX_COLUMN * 5) + 5,
            [LedId.Keyboard_C] = (KEYBOARD_MAX_COLUMN * 5) + 6,
            [LedId.Keyboard_V] = (KEYBOARD_MAX_COLUMN * 5) + 7,
            [LedId.Keyboard_B] = (KEYBOARD_MAX_COLUMN * 5) + 8,
            [LedId.Keyboard_N] = (KEYBOARD_MAX_COLUMN * 5) + 9,
            [LedId.Keyboard_M] = (KEYBOARD_MAX_COLUMN * 5) + 10,
            [LedId.Keyboard_CommaAndLessThan] = (KEYBOARD_MAX_COLUMN * 5) + 11,
            [LedId.Keyboard_PeriodAndBiggerThan] = (KEYBOARD_MAX_COLUMN * 5) + 12,
            [LedId.Keyboard_SlashAndQuestionMark] = (KEYBOARD_MAX_COLUMN * 5) + 13,
            [LedId.Keyboard_RightShift] = (KEYBOARD_MAX_COLUMN * 5) + 15,
            [LedId.Keyboard_ArrowUp] = (KEYBOARD_MAX_COLUMN * 5) + 17,
            [LedId.Keyboard_Num1] = (KEYBOARD_MAX_COLUMN * 5) + 19,
            [LedId.Keyboard_Num2] = (KEYBOARD_MAX_COLUMN * 5) + 20,
            [LedId.Keyboard_Num3] = (KEYBOARD_MAX_COLUMN * 5) + 21,
            [LedId.Keyboard_NumEnter] = (KEYBOARD_MAX_COLUMN * 5) + 22,

            #endregion

            #region Row 6

            [LedId.Keyboard_Programmable5] = (KEYBOARD_MAX_COLUMN * 6) + 1,
            [LedId.Keyboard_LeftCtrl] = (KEYBOARD_MAX_COLUMN * 6) + 2,
            [LedId.Keyboard_LeftGui] = (KEYBOARD_MAX_COLUMN * 6) + 3,
            [LedId.Keyboard_LeftAlt] = (KEYBOARD_MAX_COLUMN * 6) + 4,
            [LedId.Keyboard_Space] = (KEYBOARD_MAX_COLUMN * 6) + 7,
            [LedId.Keyboard_RightAlt] = (KEYBOARD_MAX_COLUMN * 6) + 11,
            [LedId.Keyboard_RightGui] = (KEYBOARD_MAX_COLUMN * 6) + 13,
            [LedId.Keyboard_Application] = (KEYBOARD_MAX_COLUMN * 6) + 14,
            [LedId.Keyboard_RightCtrl] = (KEYBOARD_MAX_COLUMN * 6) + 15,
            [LedId.Keyboard_ArrowLeft] = (KEYBOARD_MAX_COLUMN * 6) + 16,
            [LedId.Keyboard_ArrowDown] = (KEYBOARD_MAX_COLUMN * 6) + 17,
            [LedId.Keyboard_ArrowRight] = (KEYBOARD_MAX_COLUMN * 6) + 18,
            [LedId.Keyboard_Num0] = (KEYBOARD_MAX_COLUMN * 6) + 20,
            [LedId.Keyboard_NumPeriodAndDelete] = (KEYBOARD_MAX_COLUMN * 6) + 21,

            #endregion

            //Row 7 is also empty
        };
    }
}
