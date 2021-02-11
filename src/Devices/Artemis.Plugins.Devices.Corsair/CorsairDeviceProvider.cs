using System;
using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Devices.Corsair;

namespace Artemis.Plugins.Devices.Corsair
{
    // ReSharper disable once UnusedMember.Global
    public class CorsairDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public CorsairDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Corsair.CorsairDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Corsair.CorsairDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "CUESDK.x64_2017.dll"));
            RGB.NET.Devices.Corsair.CorsairDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "CUESDK_2017.dll"));
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void DetectDeviceLayout(ArtemisDevice rgbDevice)
        {
            if (rgbDevice.RgbDevice.DeviceInfo is CorsairKeyboardRGBDeviceInfo keyboardInfo)
            {
                // Source: https://commons.wikimedia.org/wiki/File:Physical_keyboard_layouts_comparison_ANSI_ISO_KS_ABNT_JIS.png
                switch (keyboardInfo.PhysicalLayout)
                {
                    case CorsairPhysicalKeyboardLayout.US:
                        rgbDevice.PhysicalLayout = "ANSI";
                        break;
                    case CorsairPhysicalKeyboardLayout.UK:
                        rgbDevice.PhysicalLayout = "ISO";
                        break;
                    case CorsairPhysicalKeyboardLayout.BR:
                        rgbDevice.PhysicalLayout = "ABNT";
                        break;
                    case CorsairPhysicalKeyboardLayout.JP:
                        rgbDevice.PhysicalLayout = "JIS";
                        break;
                    case CorsairPhysicalKeyboardLayout.KR:
                        rgbDevice.PhysicalLayout = "KS";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Simply use two-letter country code
                if (keyboardInfo.LogicalLayout == CorsairLogicalKeyboardLayout.US_Int)
                    rgbDevice.LogicalLayout = "US";
                else
                    rgbDevice.LogicalLayout = keyboardInfo.LogicalLayout.ToString();
            }
        }
    }
}