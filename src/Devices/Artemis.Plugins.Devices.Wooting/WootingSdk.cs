using RGB.NET.Devices.Wooting.Enum;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Artemis.Plugins.Devices.Wooting;

internal static class WootingSdk
{
    private const string DLL = "x64/wooting-rgb-sdk64.dll";

    public static int GetProfile(bool newInterface)
    {
        uint responseSize = (uint)(newInterface ? 256 : 128);
        var buffer = new byte[responseSize];
        var result = SendFeature(buffer, responseSize, GetCurrentKeyboardProfileIndex, 0, 0, 0, 0);

        if (result != responseSize)
            throw new Exception("wrong response size");

        if (newInterface)
            return buffer[5];
        else
            return buffer[4];
    }

    public const byte GetCurrentKeyboardProfileIndex = 11;

    [DllImport(DLL, EntryPoint = "wooting_usb_send_feature_with_response", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SendFeature(
        byte[] buffer,
        uint size,
        byte commandId,
        byte parameter0,
        byte parameter1,
        byte parameter2,
        byte parameter3
    );

    [DllImport(DLL, EntryPoint = "wooting_rgb_kbd_connected", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsConnected();

    [DllImport(DLL, EntryPoint = "wooting_usb_keyboard_count", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte GetKeyboardCount();

    [DllImport(DLL, EntryPoint = "wooting_usb_select_device", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SelectDevice(byte index);

    [DllImport(DLL, EntryPoint = "wooting_rgb_device_info", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetDeviceInfoPtr();

    public static WootingUsbMeta GetDeviceInfo()
    {
        return Marshal.PtrToStructure<WootingUsbMeta>(GetDeviceInfoPtr());
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct WootingUsbMeta
{
    public bool Connected { get; set; }
    
    public string Model { get; set; }

    public byte MaxRows { get; set; }

    public byte MaxColumns { get; set; }

    public byte KeycodeLimit { get; set; }

    public WootingDeviceType DeviceType { get; set; }

    public bool V2Interface { get; set; }

    public WootingLayoutType LayoutType { get; set; }
}
