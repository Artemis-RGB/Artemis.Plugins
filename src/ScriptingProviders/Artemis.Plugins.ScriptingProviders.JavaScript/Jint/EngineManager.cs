using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core.ScriptingProviders;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using Esprima;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using Serilog;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Jint
{
    public class EngineManager : IDisposable
    {
        private readonly ILogger _logger;
        private CancellationTokenSource? _cts;

        public EngineManager(Script script, ILogger logger, List<IScriptBinding> scriptBindings)
        {
            _logger = logger;

            Script = script;
            ScriptBindings = scriptBindings;
        }

        public Script Script { get; }
        public Engine? Engine { get; private set; }

        public List<IScriptBinding> ScriptBindings { get; set; }
        public List<IContextBinding> ContextBindings { get; } = new();

        public Dictionary<string, Assembly> ExtraAssemblies { get; } = new()
        {
            {"Artemis.Core", typeof(ICoreService).Assembly},
            {"SkiaSharp", typeof(SKColor).Assembly}
        };

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
                options.CancellationToken(_cts.Token);
                options.AllowClr(ExtraAssemblies.Values.ToArray());
                options.Strict(false);
            });

            // Get rid of these straight away, ain't nobody got time for that
            Engine.Global.RemoveOwnProperty(Engine.Global.GetOwnProperties().First(p => p.Key.AsString() == "System").Key);
            Engine.Global.RemoveOwnProperty(Engine.Global.GetOwnProperties().First(p => p.Key.AsString() == "importNamespace").Key);

            Engine.Execute("Artemis = {};");

            // Register extra assemblies by their namespace
            foreach ((string name, Assembly _) in ExtraAssemblies)
                Engine.SetValue(name, new NamespaceReference(Engine, name));

            foreach (IScriptBinding scriptBinding in ScriptBindings)
                scriptBinding.Initialize(Engine);
            foreach (IContextBinding contextBinding in ContextBindings) 
                contextBinding.Initialize(Engine);

            // Execute the script in a separate thread until the cancellation token is cancelled
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

            // ReSharper disable SuspiciousTypeConversion.Global
            foreach (IScriptBinding scriptBinding in ScriptBindings)
            {
                if (scriptBinding is IDisposable disposable)
                    disposable.Dispose();
            }
            foreach (IContextBinding contextBinding in ContextBindings)
            {
                if (contextBinding is IDisposable disposable)
                    disposable.Dispose();
            }
            // ReSharper restore SuspiciousTypeConversion.Global

            // If there is an engine, dispose any instances of IDisposable, hardly foolproof though
            if (Engine != null)
            {
                IEnumerable<KeyValuePair<JsValue, PropertyDescriptor>> keyValuePairs = Engine.Global.GetOwnProperties();
                foreach (var (_, propertyDescriptor) in keyValuePairs)
                {
                    if (propertyDescriptor.Value.IsObject() && propertyDescriptor.Value.ToObject() is IDisposable disposable)
                        disposable.Dispose();
                }
            }

            Engine = null;
        }

        #endregion
    }
}