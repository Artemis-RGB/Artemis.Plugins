using System;
using System.Collections.Generic;
using System.Diagnostics;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Time.DataModels;
using Artemis.Plugins.Modules.Time.Platform.Windows;

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
        DataModel.CurrentTime = DateTimeOffset.Now;
        DataModel.TimeSinceMidnight = DateTimeOffset.Now - DateTimeOffset.Now.Date;
        DataModel.TimeSinceArtemisStart = DateTimeOffset.Now - _artemisStartTime;
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
        //Hide unsupported properties until propper support is added
        if (!OperatingSystem.IsWindows())
            HideProperty(d => d.TimeSinceSystemBoot);

        DataModel.TimeSinceSystemBoot = GetTimeSinceSystemBoot();
    }

    public override void Disable()
    {
    }

    #endregion

    private static TimeSpan GetTimeSinceSystemBoot()
    {
        return Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => WindowsTimeUtils.GetTimeSinceSystemStart(),
            _ => throw new NotImplementedException()
        };
    }
}