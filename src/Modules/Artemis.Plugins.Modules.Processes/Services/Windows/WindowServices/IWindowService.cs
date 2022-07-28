using Artemis.Core.Services;

namespace Artemis.Plugins.Modules.Processes.Services
{
    public interface IWindowService
    {
        public string GetActiveWindowTitle();
        public int GetActiveProcessId();
    }
}
