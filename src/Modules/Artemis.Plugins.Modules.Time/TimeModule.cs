using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Time.DataModels;
using Artemis.Plugins.Modules.Time.Platform.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Artemis.Plugins.Modules.Time
{
    [PluginFeature(Name = "Time", Icon = "ClockTimeThree", AlwaysEnabled = true)]
    public class TimeModule : Module<TimeDataModel>
    {
        #region Variables and properties

        public override List<IModuleActivationRequirement> ActivationRequirements => null;
        private readonly DateTime _artemisStartTime;

        #endregion

        #region Constructors

        public TimeModule()
        {
            _artemisStartTime = Process.GetCurrentProcess().StartTime;
        }

        #endregion

        #region Plugin lifecicle

        public override void Enable()
        {
            //Hide unsupported properties until propper support is added
            if (!OperatingSystem.IsWindows())
            {
                HideProperty(d => d.TimeSinceSystemBoot);
            }
        }

        public override void Disable() { }

        #endregion

        #region DataModel update

        public override void Update(double deltaTime)
        {
            DataModel.CurrentTime = DateTimeOffset.Now;
            DataModel.TimeSinceMidnight = DateTimeOffset.Now - DateTimeOffset.Now.Date;

            if (OperatingSystem.IsWindows())
            {
                DataModel.TimeSinceSystemBoot = WindowsTimeUtils.GetTimeSinceSystemStart();
            }
            else if (OperatingSystem.IsLinux())
            {
                //TODO: Write Linux platform GetTimeSinceSystemStart()
            }
            else if (OperatingSystem.IsMacOS())
            {
                //TODO: Write MacOS platform GetTimeSinceSystemStart()
            }

            DataModel.TimeSinceArtemisStart = DateTimeOffset.Now - _artemisStartTime;
        }

        #endregion
    }
}