using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Artemis.Plugins.Modules.Processes.Platform.Windows;

[SupportedOSPlatform("windows")]
public static class WindowUtilities
{
    public static int GetActiveProcessId()
    {
        // Get foreground window handle
        IntPtr hWnd = GetForegroundWindow();

        _ = GetWindowThreadProcessId(hWnd, out uint processId);
        return (int) processId;
    }

    public static string GetActiveWindowTitle()
    {
        // Get foreground window handle
        IntPtr hWnd = GetForegroundWindow();

        int length = GetWindowTextLength(hWnd) + 1;
        StringBuilder title = new(length);
        _ = GetWindowText(hWnd, title, length);
        return title.ToString();
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
}