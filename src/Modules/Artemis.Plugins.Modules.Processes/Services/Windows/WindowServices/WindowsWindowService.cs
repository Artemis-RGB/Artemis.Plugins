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
}