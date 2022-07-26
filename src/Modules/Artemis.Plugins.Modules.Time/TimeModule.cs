using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Time.DataModels;
using Artemis.Plugins.Modules.Time.Services.TimeServices;
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
        private readonly ITimeService _timeServices;

        #endregion

        #region Constructors

        public TimeModule(ITimeService timeServices)
        {
            _artemisStartTime = Process.GetCurrentProcess().StartTime;
            _timeServices = timeServices;
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
            DataModel.TimeSinceSystemBoot = _timeServices.GetTimeSinceSystemStart();
            DataModel.TimeSinceArtemisStart = DateTimeOffset.Now - _artemisStartTime;
        }

        #endregion
    }
}