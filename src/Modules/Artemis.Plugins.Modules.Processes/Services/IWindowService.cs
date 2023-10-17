namespace Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

public interface IWindowService
{
    string GetActiveWindowTitle();
    int GetActiveProcessId();
    bool GetActiveWindowFullscreen();
}