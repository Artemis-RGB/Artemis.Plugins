using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual
{
    public class LayerPropertyBinding : IManualScriptBinding
    {
        private readonly ILayerProperty _layerProperty;
        private readonly Plugin _plugin;
        private readonly PluginJintEngine _pluginJintEngine;
        private readonly List<FunctionInstance> _updatedCallbacks = new();
        private readonly List<FunctionInstance> _updatingCallbacks = new();

        public LayerPropertyBinding(ILayerProperty layerProperty, Plugin plugin, PluginJintEngine pluginJintEngine)
        {
            _layerProperty = layerProperty;
            _plugin = plugin;
            _pluginJintEngine = pluginJintEngine;
        }

        internal void LayerPropertyUpdating(double deltaTime)
        {
            foreach (FunctionInstance callback in _updatingCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new JsValue[] {new JsNumber(deltaTime)});
                }
                catch (ExecutionCanceledException)
                {
                    _updatingCallbacks.Remove(callback);
                }
            }
        }

        internal void LayerPropertyUpdated(double deltaTime)
        {
            foreach (FunctionInstance callback in _updatedCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new JsValue[] {new JsNumber(deltaTime)});
                }
                catch (ExecutionCanceledException)
                {
                    _updatedCallbacks.Remove(callback);
                }
            }
        }

        #region Implementation of IManualScriptBinding

        /// <inheritdoc />
        public string Declaration => File.ReadAllText(_plugin.ResolveRelativePath("StaticDeclarations/LayerPropertyWrapper.ts"));

        #endregion

        #region JS functions

        public Action OnUpdating(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatingCallbacks.Remove(functionInstance);
        }

        public Action OnUpdated(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatedCallbacks.Remove(functionInstance);
        }

        public PropertyDescriptionAttribute Description => _layerProperty.PropertyDescription;
        public string PropertyType => _layerProperty.PropertyType.Name;
        public string Path => _layerProperty.Path;

        #endregion
    }
}