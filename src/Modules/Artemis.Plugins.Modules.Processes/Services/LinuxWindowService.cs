using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

[SupportedOSPlatform("linux")]
public class LinuxWindowService : IWindowService
{
    public int GetActiveProcessId() => 0;

    public string GetActiveWindowTitle() => string.Empty;

    public bool GetActiveWindowFullscreen() => false;
}