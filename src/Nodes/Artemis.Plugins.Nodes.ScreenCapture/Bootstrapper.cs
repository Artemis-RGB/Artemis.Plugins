using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Nodes.ScreenCapture.ScreenCapture;
using Microsoft.Win32;
using ScreenCapture.NET;

namespace Artemis.Plugins.Nodes.ScreenCapture;

public class Bootstrapper : PluginBootstrapper
{
    private Plugin? _currentPlugin;
    private IPluginManagementService? _pluginManagementService;

    public static NodeScreenCaptureService? ScreenCaptureService { get; private set; }

    public override void OnPluginEnabled(Plugin plugin)
    {
        _currentPlugin = plugin;
        _pluginManagementService = plugin.Resolve<IPluginManagementService>();

        IScreenCaptureService screenCaptureService = OperatingSystem.IsWindows() ? new DX11ScreenCaptureService() : new X11ScreenCaptureService();
        ScreenCaptureService ??= new NodeScreenCaptureService(screenCaptureService);
        if (OperatingSystem.IsWindows())
        {
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
        }
    }

    public override void OnPluginDisabled(Plugin plugin)
    {
        ScreenCaptureService?.Dispose();
        ScreenCaptureService = null;
        if (OperatingSystem.IsWindows())
        {
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;
        }
    }

    private void SystemEventsOnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        PluginFeatureInfo? nodeProvider = _currentPlugin?.GetFeatureInfo<ScreenCaptureNodesProvider>();

        // Feature not enabled
        if (nodeProvider?.Instance == null)
            return;

        _pluginManagementService?.DisablePluginFeature(nodeProvider.Instance, false);
        ScreenCaptureService?.Dispose();
        IScreenCaptureService screenCaptureService = OperatingSystem.IsWindows() ? new DX11ScreenCaptureService() : new X11ScreenCaptureService();
        ScreenCaptureService = new NodeScreenCaptureService(screenCaptureService);
        _pluginManagementService?.EnablePluginFeature(nodeProvider.Instance, false);
    }
}