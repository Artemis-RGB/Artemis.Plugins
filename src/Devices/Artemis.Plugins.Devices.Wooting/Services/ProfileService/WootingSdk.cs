using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RGB.NET.Devices.Wooting.Enum;
using Serilog;

namespace Artemis.Plugins.Devices.Wooting.Services.ProfileService;

internal static class WootingSdk
{
    public const byte GetCurrentKeyboardProfileIndex = 11;
    private const string Dll = "x64/wooting-rgb-sdk64.dll";
    private static readonly byte[] _buffer = new byte[256];
    private static readonly object _lock;

    static WootingSdk()
    {
        Type cs = typeof(RGB.NET.Devices.Wooting.WootingDeviceProvider).Assembly.GetType("RGB.NET.Devices.Wooting.Native._WootingSDK");
        _lock = cs!.GetField("SdkLock", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null);
    }

    public static bool TryGetProfile(byte deviceIndex, bool newInterface, out int profile)
    {
        int result = 0;
        lock (_lock)
        {
            wooting_usb_select_device(deviceIndex);
            result = wooting_usb_send_feature_with_response(_buffer, 256, GetCurrentKeyboardProfileIndex, 0, 0, 0, 0);
        }
        if (result != 256)
        {
            profile = -1;
            return false;
        }
        
        profile = newInterface ? _buffer[5] : _buffer[4];
        return true;
    }

    public static bool IsConnected()
    {
        lock (_lock)
        {
            return wooting_rgb_kbd_connected();
        }
    }

    public static byte GetDeviceCount()
    {
        lock (_lock)
        {
            return wooting_usb_device_count();
        }
    }

    public static WootingUsbMeta GetDeviceInfo(byte deviceIndex)
    {
        lock (_lock)
        {
            wooting_usb_select_device(deviceIndex);
            return Marshal.PtrToStructure<WootingUsbMeta>(wooting_rgb_device_info());
        }
    }

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int wooting_usb_send_feature_with_response(
        byte[] buffer,
        ulong size,
        byte commandId,
        byte parameter0,
        byte parameter1,
        byte parameter2,
        byte parameter3
    );

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool wooting_rgb_kbd_connected();

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte wooting_usb_device_count();

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool wooting_usb_select_device(byte index);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr wooting_rgb_device_info();
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

    public bool UsesSmallPackets { get; set; }
}