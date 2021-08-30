using System.Linq;
using Jint;
using Jint.Native;
using Serilog;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.ServiceBindings
{
    public class ConsoleBinding : IServiceBinding
    {
        private readonly ILogger _logger;

        public ConsoleBinding(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(params JsValue[] logObjects)
        {
            if (logObjects.Length == 0)
                _logger.Debug("undefined");
            else
                _logger.Debug(string.Join(" ", logObjects.Select(l => l.ToString())));
        }

        public void Info(params JsValue[] logObjects)
        {
            if (logObjects.Length == 0)
                _logger.Information("undefined");
            else
                _logger.Information(string.Join(" ", logObjects.Select(l => l.ToString())));
        }

        public void Warn(params JsValue[] logObjects)
        {
            if (logObjects.Length == 0)
                _logger.Warning("undefined");
            else
                _logger.Warning(string.Join(" ", logObjects.Select(l => l.ToString())));
        }

        public void Error(params JsValue[] logObjects)
        {
            if (logObjects.Length == 0)
                _logger.Error("undefined");
            else
                _logger.Error(string.Join(" ", logObjects.Select(l => l.ToString())));
        }

        public void Initialize(Engine engine)
        {
            engine.SetValue("console", this);
        }

        public string? GetDeclaration()
        {
            // No need for a declaration as console is a default API
            return null;
        }
    }
}