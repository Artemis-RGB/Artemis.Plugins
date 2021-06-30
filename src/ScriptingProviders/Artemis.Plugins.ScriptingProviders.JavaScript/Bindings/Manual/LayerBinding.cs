using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual
{
    public class LayerBinding : IManualScriptBinding
    {
        private readonly Plugin _plugin;
        private readonly PluginJintEngine _pluginJintEngine;
        private readonly Layer _layer;
        private readonly List<FunctionInstance> _renderedCallbacks = new();
        private readonly List<FunctionInstance> _renderingCallbacks = new();
        private readonly List<FunctionInstance> _updatedCallbacks = new();
        private readonly List<FunctionInstance> _updatingCallbacks = new();

        public LayerBinding(Layer layer, Plugin plugin, PluginJintEngine pluginJintEngine)
        {
            _layer = layer;
            _plugin = plugin;
            _pluginJintEngine = pluginJintEngine;
        }

        internal void LayerUpdating(double deltaTime)
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

        internal void LayerUpdated(double deltaTime)
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

        internal void LayerRendering(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (_pluginJintEngine.Engine == null)
                return;

            JsValue[] arguments =
            {
                JsValue.FromObject(_pluginJintEngine.Engine, canvas),
                JsValue.FromObject(_pluginJintEngine.Engine, bounds),
                JsValue.FromObject(_pluginJintEngine.Engine, paint)
            };
            foreach (FunctionInstance callback in _renderingCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, arguments);
                }
                catch (ExecutionCanceledException)
                {
                    _renderingCallbacks.Remove(callback);
                }
            }
        }

        internal void LayerRendered(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (_pluginJintEngine.Engine == null)
                return;

            JsValue[] arguments =
            {
                JsValue.FromObject(_pluginJintEngine.Engine, canvas),
                JsValue.FromObject(_pluginJintEngine.Engine, bounds),
                JsValue.FromObject(_pluginJintEngine.Engine, paint)
            };
            foreach (FunctionInstance callback in _renderedCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, arguments);
                }
                catch (ExecutionCanceledException)
                {
                    _renderedCallbacks.Remove(callback);
                }
            }
        }

        #region Implementation of IManualScriptBinding

        /// <inheritdoc />
        public string Declaration => File.ReadAllText(_plugin.ResolveRelativePath("StaticDeclarations/LayerWrapper.ts"));

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

        public Action OnRendering(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderingCallbacks.Remove(functionInstance);
        }

        public Action OnRendered(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderedCallbacks.Remove(functionInstance);
        }

        public Layer GetLayer()
        {
            return _layer;
        }

        #endregion
    }
}