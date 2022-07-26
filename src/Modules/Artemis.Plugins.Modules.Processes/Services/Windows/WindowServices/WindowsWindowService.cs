using Artemis.Plugins.Modules.Processes.Platform.Windows;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Processes.Services
{
    [SupportedOSPlatform("windows")]
    public class WindowsWindowService : IWindowService
    {
        public string GetActiveWindowTitle()
        {
            return WindowUtilities.GetActiveWindowTitle();
        }
    }
}
