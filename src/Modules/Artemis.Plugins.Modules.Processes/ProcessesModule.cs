using System;
using System.Collections.Generic;
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
        IWindowService windowService,
        ILogger logger)
    {
        _windowService = windowService;
        _logger = logger;
        _enableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
        _cache = new();
    }

    #endregion

    #region Variables and properties

    private readonly Dictionary<string, ColorSwatch> _cache;
    private readonly PluginSetting<bool> _enableActiveWindow;
    private readonly IWindowService _windowService;
    private readonly ILogger _logger;
    private int _lastForegroundWindowPid;

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
        DataModel.RunningProcesses = ProcessMonitor.Processes.Select(p => p.ProcessName).Except(Constants.IgnoredWindowsProcessList).ToList();
    }

    private void UpdateCurrentWindow(double deltaTime)
    {
        if (!_enableActiveWindow.Value)
            return;

        int foregroundWindowPid = _windowService.GetActiveProcessId();
        bool hasChanged = foregroundWindowPid != _lastForegroundWindowPid;
        _lastForegroundWindowPid = foregroundWindowPid;

        DataModel.ActiveWindow.IsFullscreen = _windowService.GetActiveWindowFullscreen();
        if (!hasChanged)
            return;

        ProcessInfo? foregroundProcess = ProcessMonitor.Processes.Cast<ProcessInfo?>().FirstOrDefault(p => p!.Value.ProcessId == foregroundWindowPid, null);
        if (foregroundProcess == null)
            return;

        DataModel.ActiveWindow.WindowTitle = _windowService.GetActiveWindowTitle();
        DataModel.ActiveWindow.ProcessName = foregroundProcess.Value.ProcessName;
        DataModel.ActiveWindow.ProgramLocation = foregroundProcess.Value.Executable;

        try
        {
            DataModel.ActiveWindow.Colors = GetOrComputeSwatch(DataModel.ActiveWindow.ProgramLocation) ?? default;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to compute color swatch for {ProcessName}", foregroundProcess.Value.ProcessName);
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