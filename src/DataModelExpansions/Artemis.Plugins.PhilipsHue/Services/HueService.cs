using System;
using System.Diagnostics;
using Artemis.Core;
using Artemis.Core.Services;

namespace Artemis.Plugins.PhilipsHue.Services
{
    public class HueService : IHueService
    {
        private readonly Plugin _plugin;

        public HueService(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void Dispose()
        {
            Debug.WriteLine("Hue service is being disposed :)");
        }
    }

    public interface IHueService : IPluginService, IDisposable
    {
    }
}