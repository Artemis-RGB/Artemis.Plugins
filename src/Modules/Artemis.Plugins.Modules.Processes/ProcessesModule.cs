using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Processes.DataModels;
using Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

namespace Artemis.Plugins.Modules.Processes;

[PluginFeature(Name = "Processes", Icon = "Application", AlwaysEnabled = true)]
public class ProcessesModule : Module<ProcessesDataModel>
{
    #region Constructors

    public ProcessesModule(IColorQuantizerService quantizerService, PluginSettings settings, IProcessMonitorService processMonitorService, IWindowService windowService)
    {
        _quantizerService = quantizerService;
        _processMonitorService = processMonitorService;
        _windowService = windowService;
        _enableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
    }

    #endregion

    #region Open windows

    public void UpdateCurrentWindow()
    {
        if (!_enableActiveWindow.Value)
            return;

        int processId = _windowService.GetActiveProcessId();
        if (DataModel.ActiveWindow == null || DataModel.ActiveWindow.Process.Id != processId)
            DataModel.ActiveWindow = new WindowDataModel(Process.GetProcessById(processId), _quantizerService, _windowService);

        DataModel.ActiveWindow?.UpdateWindowTitle();
    }

    #endregion

    #region Variables and properties

    private readonly PluginSetting<bool> _enableActiveWindow;
    private readonly IColorQuantizerService _quantizerService;
    private readonly IProcessMonitorService _processMonitorService;
    private readonly IWindowService _windowService;

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    #endregion

    #region Plugin lifecicle

    public override void Enable()
    {
        _enableActiveWindow.SettingChanged += EnableActiveWindowOnSettingChanged;

        AddTimedUpdate(TimeSpan.FromMilliseconds(250), _ => UpdateCurrentWindow(), "UpdateCurrentWindow");
        AddTimedUpdate(TimeSpan.FromSeconds(1), _ => UpdateRunningProcesses(), "UpdateRunningProcesses");
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

    private void UpdateRunningProcesses()
    {
        DataModel.RunningProcesses = _processMonitorService.GetRunningProcesses().Select(p => p.ProcessName).Except(Constants.IgnoredWindowsProcessList).ToList();
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

    #endregion
}