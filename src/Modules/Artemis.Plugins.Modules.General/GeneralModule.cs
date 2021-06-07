using System;
using System.Diagnostics;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.General.DataModels;
using Artemis.Plugins.Modules.General.DataModels.Windows;
using Artemis.Plugins.Modules.General.Utilities;

namespace Artemis.Plugins.Modules.General
{
    [PluginFeature(AlwaysEnabled = true)]
    public class GeneralModule : Module<GeneralDataModel>
    {
        private readonly PluginSetting<bool> _enableActiveWindow;
        private readonly PluginSetting<bool> _disableDefaultProfilesCreation;
        private readonly IColorQuantizerService _quantizerService;

        public GeneralModule(IColorQuantizerService quantizerService, PluginSettings settings)
        {
            _quantizerService = quantizerService;
            _enableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
            _disableDefaultProfilesCreation = settings.GetSetting("DisableDefaultProfilesCreation", false);

            DisplayName = "General";
            DisplayIcon = "Images/bow.svg";

            if (!_disableDefaultProfilesCreation.Value)
            {
                AddDefaultProfile(DefaultCategoryName.General, "Profiles/rainbow.json");
                AddDefaultProfile(DefaultCategoryName.General, "Profiles/noise.json");
            }
        }

        public override void Enable()
        {
            _enableActiveWindow.SettingChanged += EnableActiveWindowOnSettingChanged;

            AddTimedUpdate(TimeSpan.FromMilliseconds(250), _ => UpdateCurrentWindow(), "UpdateCurrentWindow");
            AddTimedUpdate(TimeSpan.FromSeconds(1.5), _ => UpdatePerformance(), "UpdatePerformance");

            ApplyEnableActiveWindow();
        }

        public override void Disable()
        {
            _enableActiveWindow.SettingChanged -= EnableActiveWindowOnSettingChanged;
        }

        public override void Update(double deltaTime)
        {
            DataModel.TimeDataModel.CurrentTime = DateTimeOffset.Now;
            DataModel.TimeDataModel.TimeSinceMidnight = DateTimeOffset.Now - DateTimeOffset.Now.Date;
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

        private void UpdatePerformance()
        {
            DataModel.PerformanceDataModel.CpuUsage = Performance.GetCpuUsage();
            DataModel.PerformanceDataModel.AvailableRam = Performance.GetPhysicalAvailableMemoryInMiB();
            DataModel.PerformanceDataModel.TotalRam = Performance.GetTotalMemoryInMiB();
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
    }
}