using System;
using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.Time.DataModels;

public class TimeDataModel : DataModel
{
    [DataModelProperty(Name = "Current")]
    public DateTimeOffset CurrentTime { get; set; }

    public TimeSpan TimeSinceMidnight { get; set; }
    public TimeSpan TimeSinceSystemBoot { get; set; }
    public TimeSpan TimeSinceArtemisStart { get; set; }
}