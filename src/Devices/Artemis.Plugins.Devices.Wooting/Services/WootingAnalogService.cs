using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WootingAnalogSDKNET;
using RGB.NET.Core;
using System.Collections.ObjectModel;
using Artemis.Core;
using Artemis.Core.Services;
using System.Collections.Concurrent;

namespace Artemis.Plugins.Devices.Wooting
{
    public class WootingAnalogService : IPluginService, IDisposable
    {
        private readonly ConcurrentDictionary<LedId, float> _analogValues;

        public IReadOnlyDictionary<LedId, float> Values { get; }

        public WootingAnalogService()
        {
            _analogValues = new();
            Values = new ReadOnlyDictionary<LedId, float>(_analogValues);

            var (count, result) = WootingAnalogSDK.Initialise();

            if (count < 1 || result != WootingAnalogResult.Ok)
                throw new Exception();

            WootingAnalogSDK.SetKeycodeMode(KeycodeType.HID);

            _reversed = _HIDCodes.ToDictionary(k => (short)k.Value, s => s.Key);
        }

        public void Update()
        {
            var (data, res) = WootingAnalogSDK.ReadFullBuffer();
            foreach (var item in data)
            {
                if (_reversed.TryGetValue(item.Item1, out var ledId))
                {
                    _analogValues[ledId] = item.Item2;
                }
                else
                {
                    Console.WriteLine(item.Item1);
                }
            }

            //foreach ((var ledId, var virtualShortCode) in _virtualKeyCodes)
            //{
            //    var (analogValue, analogReadResult) = WootingAnalogSDK.ReadAnalog(virtualShortCode);

            //    if (analogReadResult == WootingAnalogResult.NoMapping)
            //        continue;

            //    if (analogReadResult != WootingAnalogResult.Ok)
            //        throw new InvalidOperationException();

            //    _analogValues[ledId] = analogValue;
            //}
        }

        private readonly Dictionary<LedId, ushort> _HIDCodes = new()
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

            /**
             * Thanks Simon for handing me this list
             * RgbBrightnessUp = 0x401
             * RgbBrightnessDown = 0x402
             * SelectAnalogProfile1 = 0x403
             * SelectAnalogProfile2 = 0x404
             * SelectAnalogProfile3 = 0x405
             * ModeKey = 0x408
             * FnKey = 0x409
             * DisableKey = 0x40A
             * FnLayerLock = 0x40B
             * FnKey2 = 0x40C= 
             */
            [LedId.Keyboard_Custom1] = 1027,
            [LedId.Keyboard_Custom2] = 1028,
            [LedId.Keyboard_Custom3] = 1029,
            [LedId.Keyboard_Custom4] = 1032,
            [LedId.Keyboard_Function] = 1033
        };

        private Dictionary<short, LedId> _reversed;
        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WootingAnalogSDK.UnInitialise();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
