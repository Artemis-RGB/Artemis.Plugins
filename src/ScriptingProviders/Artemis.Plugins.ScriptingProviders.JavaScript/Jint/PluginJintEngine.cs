using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Esprima;
using Jint;
using Jint.Runtime;
using Ninject;
using Ninject.Parameters;
using Serilog;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Jint
{
    public class PluginJintEngine : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Plugin _plugin;
        private CancellationTokenSource _cts;

        public PluginJintEngine(Script script, Plugin plugin, ILogger logger)
        {
            _plugin = plugin;
            _logger = logger;

            Script = script;
            ExecuteScript();
        }

        public Script Script { get; }
        public Engine Engine { get; private set; }
        public Dictionary<string, object> ExtraValues { get; } = new();

        /// <summary>
        ///     Disposes the old engine, creates a fresh engine and executes the current script in it
        /// </summary>
        public void ExecuteScript()
        {
            Dispose();

            if (Script.ScriptConfiguration.ScriptContent == null)
                return;

            _cts = new CancellationTokenSource();
            Engine = new Engine(options =>
            {
                // Limit memory allocations to 100 MB
                // options.LimitMemory(100_000_000);
                options.CancellationToken(_cts.Token);
                options.AllowClr(typeof(SKCanvas).Assembly);
            });

            Engine.Execute("const SkiaSharp = importNamespace('SkiaSharp')");

            List<IScriptBinding> scriptBindings = _plugin.Kernel!.GetAll<IScriptBinding>(new ConstructorArgument("engine", this)).ToList();
            foreach (IScriptBinding scriptBinding in scriptBindings)
                Engine.SetValue(scriptBinding.Name, scriptBinding);
            foreach ((string key, object value) in ExtraValues) 
                Engine.SetValue(key, value);

            Task.Run(() =>
            {
                try
                {
                    Engine.Execute(Script.ScriptConfiguration.ScriptContent, new ParserOptions(Script.ScriptConfiguration.Name));
                }
                catch (ExecutionCanceledException)
                {
                    // ignored
                }
                catch (Exception e)
                {
                    _logger.Error(e, "JavaScript engine error");
                }
            }, _cts.Token);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            Engine = null;
        }

        #endregion
    }
}