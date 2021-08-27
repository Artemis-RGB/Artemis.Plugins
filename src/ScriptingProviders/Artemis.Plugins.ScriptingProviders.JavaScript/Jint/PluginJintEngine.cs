using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual;
using Artemis.Plugins.ScriptingProviders.JavaScript.Utilities;
using Esprima;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
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
        private CancellationTokenSource? _cts;

        public PluginJintEngine(Script script, Plugin plugin, ILogger logger)
        {
            _plugin = plugin;
            _logger = logger;

            Script = script;
            ExecuteScript();
        }

        public Script Script { get; }
        public Engine? Engine { get; private set; }
        public Dictionary<string, IManualScriptBinding> ExtraValues { get; } = new();

        public Dictionary<string, Assembly> ExtraAssemblies { get; } = new()
        {
            {"Artemis.Core", typeof(Profile).Assembly},
            {"SkiaSharp", typeof(SKColor).Assembly}
        };

        public List<Type> ExtraTypes { get; } = new()
        {
            typeof(TimeSpan),
            typeof(EasingNumber),
            typeof(Audio)
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

            // Register extra assemblies by their namespace
            foreach ((string name, Assembly _) in ExtraAssemblies)
                Engine.SetValue(name, new NamespaceReference(Engine, name));

            // Register script bindings
            List<IScriptBinding> scriptBindings = _plugin.Kernel!.GetAll<IScriptBinding>(new ConstructorArgument("engine", this)).ToList();
            foreach (IScriptBinding scriptBinding in scriptBindings.Where(b => b.Name != null))
                Engine.SetValue(scriptBinding.Name, scriptBinding);

            // Register extra values, whatever they may be
            foreach ((string key, object value) in ExtraValues)
                Engine.SetValue(key, value);
            foreach (Type extraType in ExtraTypes)
                Engine.SetValue(extraType.Name, TypeReference.CreateTypeReference(Engine, extraType));

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

            // If there is an engine, dispose any instances of IDisposable
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