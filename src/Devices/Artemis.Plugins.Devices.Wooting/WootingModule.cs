using Artemis.Core.Modules;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Artemis.Plugins.Devices.Wooting
{
    public class WootingModule : Module<WootingDataModel>
    {
        private readonly WootingAnalogService _service;
        private readonly Dictionary<LedId, DynamicChild<float>> _cache;

        public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

        public WootingModule(WootingAnalogService service)
        {
            _service = service;
            _cache = new();
        }

        public override void Enable()
        {
            AddTimedUpdate(TimeSpan.FromMilliseconds(250), _ =>
            {
                if (!WootingImports.wooting_rgb_kbd_connected())
                    return;

                DataModel.Profile = WootingImports.GetProfile();
            }, "Update keyboard profile");
        }

        public override void Update(double deltaTime)
        {
            UpdateAnalogValues();
        }

        public override void Disable()
        {
        }

        private void UpdateAnalogValues()
        {
            _service.Update();
            foreach (var item in _service.Values)
            {
                if (!_cache.TryGetValue(item.Key, out var keyDataModel))
                {
                    keyDataModel = DataModel.Analog.AddDynamicChild(item.Key.ToString(), item.Value);
                    _cache.Add(item.Key, keyDataModel);
                }

                keyDataModel.Value = item.Value;
            }
        }
    }

    internal static class WootingImports
    {
        public static int GetProfile()
        {
            var buffer = new byte[256];
            wooting_usb_send_feature_with_response(buffer, 256, GetCurrentKeyboardProfileIndex, 0, 0, 0, 0);

            return buffer[4];
        }

        public const byte GetCurrentKeyboardProfileIndex = 11;

        [DllImport("wooting-rgb-sdk64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wooting_usb_send_feature_with_response(
            byte[] buffer,
            uint size,
            byte commandId,
            byte parameter0,
            byte parameter1,
            byte parameter2,
            byte parameter3
        );

        [DllImport("wooting-rgb-sdk64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool wooting_rgb_kbd_connected();
    }
}
