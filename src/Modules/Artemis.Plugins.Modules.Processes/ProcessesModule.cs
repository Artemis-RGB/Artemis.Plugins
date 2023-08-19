using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Core.ColorScience;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Processes.DataModels;
using Artemis.Plugins.Modules.Processes.Platform.Windows;
using Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;
using Serilog;
using SkiaSharp;

namespace Artemis.Plugins.Modules.Processes;

[PluginFeature(Name = "Processes", AlwaysEnabled = true)]
public class ProcessesModule : Module<ProcessesDataModel>
{
    #region Constructors

    public ProcessesModule(
        PluginSettings settings,
        IProcessMonitorService processMonitorService,
        IWindowService windowService,
        ILogger logger)
    {
        _processMonitorService = processMonitorService;
        _windowService = windowService;
        _logger = logger;
        _enableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
        _cache = new();
    }

    #endregion

    #region Variables and properties

    private readonly Dictionary<string, ColorSwatch> _cache;
    private readonly PluginSetting<bool> _enableActiveWindow;
    private readonly IProcessMonitorService _processMonitorService;
    private readonly IWindowService _windowService;
    private readonly ILogger _logger;

    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

    #endregion

    #region Plugin lifecicle

    public override void Enable()
    {
        _enableActiveWindow.SettingChanged += EnableActiveWindowOnSettingChanged;

        AddTimedUpdate(TimeSpan.FromMilliseconds(250), UpdateCurrentWindow);
        AddTimedUpdate(TimeSpan.FromSeconds(1), UpdateRunningProcesses);
        ApplyEnableActiveWindow();
    }

    public override void Disable()
    {
        _enableActiveWindow.SettingChanged -= EnableActiveWindowOnSettingChanged;
    }

    #endregion

    #region DataModel update

    public override void Update(double deltaTime)
    {
    }

    private void UpdateRunningProcesses(double deltaTime)
    {
        DataModel.RunningProcesses = _processMonitorService.GetRunningProcesses().Select(p => p.ProcessName).Except(Constants.IgnoredWindowsProcessList).ToList();
    }

    private void UpdateCurrentWindow(double deltaTime)
    {
        if (!_enableActiveWindow.Value)
            return;

        int foregroundWindowPid = _windowService.GetActiveProcessId();
        Process foregroundProcess = _processMonitorService.GetRunningProcesses().FirstOrDefault(p => p.Id == foregroundWindowPid);
        if (foregroundProcess == null)
            return;

        DataModel.ActiveWindow.WindowTitle = _windowService.GetActiveWindowTitle();
        DataModel.ActiveWindow.ProcessName = foregroundProcess.ProcessName;
        DataModel.ActiveWindow.ProgramLocation = foregroundProcess.GetProcessFilename();
        DataModel.ActiveWindow.IsFullscreen = WindowUtilities.GetUserNotificationState()
            is WindowUtilities.UserNotificationState.QUNS_BUSY
            or WindowUtilities.UserNotificationState.QUNS_RUNNING_D3D_FULL_SCREEN
            or WindowUtilities.UserNotificationState.QUNS_PRESENTATION_MODE;

        try
        {
            DataModel.ActiveWindow.Colors = GetOrComputeSwatch(DataModel.ActiveWindow.ProgramLocation) ?? default;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to compute color swatch for {ProcessName}", foregroundProcess.ProcessName);
        }
    }

    private void EnableActiveWindowOnSettingChanged(object sender, EventArgs e)
    {
        ApplyEnableActiveWindow();
    }

    private void ApplyEnableActiveWindow()
    {
        if (_enableActiveWindow.Value)
            ShowProperty(d => d.ActiveWindow);
        else
            HideProperty(d => d.ActiveWindow);
    }

    private ColorSwatch? GetOrComputeSwatch(string location)
    {
        if (!_cache.TryGetValue(location, out ColorSwatch swatch))
        {
            if (!File.Exists(location))
                return null;

            using MemoryStream stream = new();
            Icon.ExtractAssociatedIcon(location)?.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using SKBitmap bitmap = SKBitmap.FromImage(SKImage.FromEncodedData(stream));
            stream.Close();
            SKColor[] colors = ColorQuantizer.Quantize(bitmap.Pixels, 256);
            swatch = ColorQuantizer.FindAllColorVariations(colors, true);
            _cache[location] = swatch;
        }

        return swatch;
    }

    #endregion
}