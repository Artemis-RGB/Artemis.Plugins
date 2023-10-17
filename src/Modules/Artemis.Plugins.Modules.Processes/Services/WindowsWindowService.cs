using System.Runtime.Versioning;
using Artemis.Plugins.Modules.Processes.Platform.Windows;

namespace Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

[SupportedOSPlatform("windows")]
public class WindowsWindowService : IWindowService
{
    public int GetActiveProcessId()
    {
        return WindowUtilities.GetActiveProcessId();
    }

    public string GetActiveWindowTitle()
    {
        return WindowUtilities.GetActiveWindowTitle();
    }

    public bool GetActiveWindowFullscreen()
    {
        return WindowUtilities.GetUserNotificationState()
            is WindowUtilities.UserNotificationState.QUNS_BUSY
            or WindowUtilities.UserNotificationState.QUNS_RUNNING_D3D_FULL_SCREEN
            or WindowUtilities.UserNotificationState.QUNS_PRESENTATION_MODE;
    }
}