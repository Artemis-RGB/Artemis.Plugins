using Artemis.Core.Services;
using System;

namespace Artemis.Plugins.Modules.Time.Services.TimeServices
{
    public interface ITimeService
    {
        public TimeSpan GetTimeSinceSystemStart();
    }
}
