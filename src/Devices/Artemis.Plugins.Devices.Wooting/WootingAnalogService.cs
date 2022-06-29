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

namespace Artemis.Plugins.Devices.Wooting
{
    public class WootingAnalogService : IPluginService, IDisposable
    {
        private readonly Dictionary<LedId, float> _analogValues;

        public IReadOnlyDictionary<LedId, float> Values { get; }

        public WootingAnalogService(Plugin profiler)
        {
            _analogValues = new();
            Values = new ReadOnlyDictionary<LedId, float>(_analogValues);

            var (count, result) = WootingAnalogSDK.Initialise();

            if (count < 1 || result != WootingAnalogResult.Ok)
                throw new Exception();

            WootingAnalogSDK.SetKeycodeMode(KeycodeType.VirtualKey);
            Update();
        }

        public void Update()
        {
            foreach ((var ledId, var virtualShortCode) in _virtualKeyCodes)
            {
                var (analogValue, analogReadResult) = WootingAnalogSDK.ReadAnalog(virtualShortCode);

                if (analogReadResult == WootingAnalogResult.NoMapping)
                    continue;

                if (analogReadResult != WootingAnalogResult.Ok)
                    throw new InvalidOperationException();

                _analogValues[ledId] = analogValue;
            }
        }

        private readonly Dictionary<LedId, ushort> _virtualKeyCodes = new Dictionary<LedId, ushort>()
        {
            [LedId.Keyboard_Backspace] = 8,
            [LedId.Keyboard_Tab] = 9,
            [LedId.Keyboard_Enter] = 13,
            [LedId.Keyboard_LeftShift] = 16,
            [LedId.Keyboard_LeftCtrl] = 17,
            [LedId.Keyboard_LeftAlt] = 18,
            [LedId.Keyboard_PauseBreak] = 19,
            [LedId.Keyboard_CapsLock] = 20,
            [LedId.Keyboard_Escape] = 27,
            [LedId.Keyboard_Space] = 32,
            [LedId.Keyboard_PageUp] = 33,
            [LedId.Keyboard_PageDown] = 34,
            [LedId.Keyboard_End] = 35,
            [LedId.Keyboard_Home] = 36,
            [LedId.Keyboard_ArrowLeft] = 37,
            [LedId.Keyboard_ArrowUp] = 38,
            [LedId.Keyboard_ArrowRight] = 39,
            [LedId.Keyboard_ArrowDown] = 40,
            [LedId.Keyboard_PrintScreen] = 44,
            [LedId.Keyboard_Insert] = 45,
            [LedId.Keyboard_Delete] = 46,
            [LedId.Keyboard_0] = 48,
            [LedId.Keyboard_1] = 49,
            [LedId.Keyboard_2] = 50,
            [LedId.Keyboard_3] = 51,
            [LedId.Keyboard_4] = 52,
            [LedId.Keyboard_5] = 53,
            [LedId.Keyboard_6] = 54,
            [LedId.Keyboard_7] = 55,
            [LedId.Keyboard_8] = 56,
            [LedId.Keyboard_9] = 57,
            [LedId.Keyboard_A] = 65,
            [LedId.Keyboard_B] = 66,
            [LedId.Keyboard_C] = 67,
            [LedId.Keyboard_D] = 68,
            [LedId.Keyboard_E] = 69,
            [LedId.Keyboard_F] = 70,
            [LedId.Keyboard_G] = 71,
            [LedId.Keyboard_H] = 72,
            [LedId.Keyboard_I] = 73,
            [LedId.Keyboard_J] = 74,
            [LedId.Keyboard_K] = 75,
            [LedId.Keyboard_L] = 76,
            [LedId.Keyboard_M] = 77,
            [LedId.Keyboard_N] = 78,
            [LedId.Keyboard_O] = 79,
            [LedId.Keyboard_P] = 80,
            [LedId.Keyboard_Q] = 81,
            [LedId.Keyboard_R] = 82,
            [LedId.Keyboard_S] = 83,
            [LedId.Keyboard_T] = 84,
            [LedId.Keyboard_U] = 85,
            [LedId.Keyboard_V] = 86,
            [LedId.Keyboard_W] = 87,
            [LedId.Keyboard_X] = 88,
            [LedId.Keyboard_Y] = 89,
            [LedId.Keyboard_Z] = 90,
            [LedId.Keyboard_LeftGui] = 91,
            [LedId.Keyboard_RightGui] = 92,
            [LedId.Keyboard_Application] = 93,
            [LedId.Keyboard_Num0] = 96,
            [LedId.Keyboard_Num1] = 97,
            [LedId.Keyboard_Num2] = 98,
            [LedId.Keyboard_Num3] = 99,
            [LedId.Keyboard_Num4] = 100,
            [LedId.Keyboard_Num5] = 101,
            [LedId.Keyboard_Num6] = 102,
            [LedId.Keyboard_Num7] = 103,
            [LedId.Keyboard_Num8] = 104,
            [LedId.Keyboard_Num9] = 105,
            [LedId.Keyboard_NumAsterisk] = 106,
            [LedId.Keyboard_NumPlus] = 107,
            [LedId.Keyboard_NumMinus] = 109,
            [LedId.Keyboard_NumPeriodAndDelete] = 110,
            [LedId.Keyboard_NumSlash] = 111,
            [LedId.Keyboard_F1] = 112,
            [LedId.Keyboard_F2] = 113,
            [LedId.Keyboard_F3] = 114,
            [LedId.Keyboard_F4] = 115,
            [LedId.Keyboard_F5] = 116,
            [LedId.Keyboard_F6] = 117,
            [LedId.Keyboard_F7] = 118,
            [LedId.Keyboard_F8] = 119,
            [LedId.Keyboard_F9] = 120,
            [LedId.Keyboard_F10] = 121,
            [LedId.Keyboard_F11] = 122,
            [LedId.Keyboard_F12] = 123,
        };

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
