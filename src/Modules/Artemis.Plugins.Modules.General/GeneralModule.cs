using System;
using System.Collections.Generic;
using System.Diagnostics;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.General.DataModels;
using Artemis.Plugins.Modules.General.DataModels.Windows;
using Artemis.Plugins.Modules.General.Utilities;
using Artemis.Plugins.Modules.General.ViewModels;
using SkiaSharp;

namespace Artemis.Plugins.Modules.General
{
    [PluginFeature(AlwaysEnabled = true)]
    public class GeneralModule : ProfileModule<GeneralDataModel>
    {
        private readonly IColorQuantizerService _quantizerService;
        private readonly PluginSetting<bool> _enableActiveWindow;

        public GeneralModule(IColorQuantizerService quantizerService, PluginSettings settings)
        {
            _quantizerService = quantizerService;
            _enableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
        }

        public override void Enable()
        {
            _enableActiveWindow.SettingChanged += EnableActiveWindowOnSettingChanged;

            DisplayName = "General";
            DisplayIcon = "Images/bow.svg";
            ExpandsDataModel = true;

            ModuleTabs = new List<ModuleTab> {new ModuleTab<GeneralViewModel>("General")};
            AddTimedUpdate(TimeSpan.FromMilliseconds(250), _ => UpdateCurrentWindow());
            AddTimedUpdate(TimeSpan.FromSeconds(1.5), _ => UpdatePerformance());

            ApplyEnableActiveWindow();
        }

        private void UpdatePerformance()
        {
            DataModel.PerformanceDataModel.CpuUsage = Performance.GetCpuUsage();
            DataModel.PerformanceDataModel.AvailableRam = Performance.GetPhysicalAvailableMemoryInMiB();
            DataModel.PerformanceDataModel.TotalRam = Performance.GetTotalMemoryInMiB();
        }

        public override void Disable()
        {
            _enableActiveWindow.SettingChanged -= EnableActiveWindowOnSettingChanged;
        }

        public override void ModuleActivated(bool isOverride)
        {
        }

        public override void ModuleDeactivated(bool isOverride)
        {
        }

        public override void Update(double deltaTime)
        {
            DataModel.TimeDataModel.CurrentTime = DateTimeOffset.Now;
            DataModel.TimeDataModel.TimeSinceMidnight = DateTimeOffset.Now - DateTimeOffset.Now.Date;
        }

        public override void Render(double deltaTime, SKCanvas canvas, SKImageInfo canvasInfo)
        {
        }

        #region Open windows

        public void UpdateCurrentWindow()
        {
            if (!_enableActiveWindow.Value)
                return;

            int processId = WindowUtilities.GetActiveProcessId();
            if (DataModel.ActiveWindow == null || DataModel.ActiveWindow.Process.Id != processId)
                DataModel.ActiveWindow = new WindowDataModel(Process.GetProcessById(processId), _quantizerService);

            DataModel.ActiveWindow?.UpdateWindowTitle();
        }

        #endregion

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
    }
}