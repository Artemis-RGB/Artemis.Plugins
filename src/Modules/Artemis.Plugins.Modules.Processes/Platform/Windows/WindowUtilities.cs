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
        IntPtr hWnd = GetForegroundWindow();
        _ = GetWindowThreadProcessId(hWnd, out uint processId);
        return (int) processId;
    }

    public static string GetActiveWindowTitle()
    {
        IntPtr hWnd = GetForegroundWindow();

        int length = GetWindowTextLength(hWnd) + 1;
        StringBuilder title = new(length);
        _ = GetWindowText(hWnd, title, length);
        return title.ToString();
    }

    public static UserNotificationState GetUserNotificationState()
    {
        SHQueryUserNotificationState(out UserNotificationState state);
        return state;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("shell32.dll")]
    private static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);

    public enum UserNotificationState
    {
        QUNS_NOT_PRESENT = 1,
        QUNS_BUSY = 2,
        QUNS_RUNNING_D3D_FULL_SCREEN = 3,
        QUNS_PRESENTATION_MODE = 4,
        QUNS_ACCEPTS_NOTIFICATIONS = 5,
        QUNS_QUIET_TIME = 6
    }
}