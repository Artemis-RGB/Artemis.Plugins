using Artemis.Core.Modules;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.Wooting
{
    public class WootingModule : Module<WootingDataModel>
    {
        public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

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
        }

        public override void Disable()
        {
        }
    }

    public class WootingDataModel : DataModel
    {
        public int Profile { get; set; }

        public bool IsInAnalogProfile => Profile != 0;

        public bool IsInDigitalProfile => Profile == 0;
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
