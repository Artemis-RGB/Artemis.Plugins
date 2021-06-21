using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Serilog;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public class ConsoleBinding : IScriptBinding
    {
        private readonly ILogger _logger;

        public ConsoleBinding(PluginJintEngine engine, ILogger logger)
        {
            _logger = logger;
        }

        public void Log(object logObject)
        {
            _logger.Debug(logObject?.ToString());
        }

        public void Info(object logObject)
        {
            _logger.Information(logObject?.ToString());
        }

        public void Warn(object logObject)
        {
            _logger.Warning(logObject?.ToString());
        }

        public void Error(object logObject)
        {
            _logger.Error(logObject?.ToString());
        }

        public string Name => "console";
    }

    public interface IScriptBinding
    {
        public string Name { get; }
    }
}