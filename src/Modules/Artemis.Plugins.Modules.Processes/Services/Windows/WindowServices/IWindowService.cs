namespace Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

public interface IWindowService
{
    public string GetActiveWindowTitle();
    public int GetActiveProcessId();
}