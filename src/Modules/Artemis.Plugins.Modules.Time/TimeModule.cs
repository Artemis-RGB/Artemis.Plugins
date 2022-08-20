using System;
using System.Collections.Generic;
using System.Diagnostics;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Time.DataModels;

namespace Artemis.Plugins.Modules.Time;

[PluginFeature(Name = "Time", Icon = "ClockTimeThree", AlwaysEnabled = true)]
public class TimeModule : Module<TimeDataModel>
{
    #region Constructors

    public TimeModule()
    {
        _artemisStartTime = Process.GetCurrentProcess().StartTime;
    }

    #endregion

    #region DataModel update

    public override void Update(double deltaTime)
    {
        DateTime now = DateTime.Now;
        DataModel.CurrentTime = now;
        DataModel.TimeSinceMidnight = now - now.Date;
        DataModel.TimeSinceArtemisStart = now - _artemisStartTime;
        DataModel.TimeSinceSystemBoot += TimeSpan.FromSeconds(deltaTime);
    }

    #endregion

    #region Variables and properties

    public override List<IModuleActivationRequirement> ActivationRequirements => null;
    private readonly DateTime _artemisStartTime;

    #endregion

    #region Plugin lifecicle

    public override void Enable()
    {
        DataModel.TimeSinceSystemBoot = TimeSpan.FromMilliseconds(Environment.TickCount64);
    }

    public override void Disable()
    {
    }

    #endregion
}