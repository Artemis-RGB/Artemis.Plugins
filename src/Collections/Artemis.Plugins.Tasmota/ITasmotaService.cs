using Artemis.Core;
using Artemis.Core.Services;
using Stylet.Logging;
using System;

namespace Artemis.Plugins.Tasmota
{

    public class TasmotaService : ITasmotaService
    {
        private readonly ILogger _logger;

        public TasmotaService(ILogger logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
        }
    }

    public interface ITasmotaService : IPluginService, IDisposable
    {
    }
}