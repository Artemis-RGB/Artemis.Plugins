using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual
{
    public class ProfileBinding
    {
        private readonly PluginJintEngine _pluginJintEngine;
        private readonly Profile _profile;
        private readonly List<FunctionInstance> _renderedCallbacks = new();
        private readonly List<FunctionInstance> _renderingCallbacks = new();
        private readonly List<FunctionInstance> _updatedCallbacks = new();

        private readonly List<FunctionInstance> _updatingCallbacks = new();

        public ProfileBinding(Profile profile, PluginJintEngine pluginJintEngine)
        {
            _profile = profile;
            _pluginJintEngine = pluginJintEngine;
        }

        // ReSharper disable once InconsistentNaming
        public Action onUpdating(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatingCallbacks.Remove(functionInstance);
        }

        // ReSharper disable once InconsistentNaming
        public Action onUpdated(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatedCallbacks.Remove(functionInstance);
        }

        // ReSharper disable once InconsistentNaming
        public Action onRendering(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderingCallbacks.Remove(functionInstance);
        }

        // ReSharper disable once InconsistentNaming
        public Action onRendered(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderedCallbacks.Remove(functionInstance);
        }

        internal void ProfileUpdating(double deltaTime)
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

        internal void ProfileUpdated(double deltaTime)
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

        internal void ProfileRendering(SKCanvas canvas, SKRect bounds)
        {
            JsValue canvasValue = JsValue.FromObject(_pluginJintEngine.Engine, canvas);
            JsValue boundsValue = JsValue.FromObject(_pluginJintEngine.Engine, bounds);
            foreach (FunctionInstance callback in _renderingCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new[] {canvasValue, boundsValue});
                }
                catch (ExecutionCanceledException)
                {
                    _renderingCallbacks.Remove(callback);
                }
            }
        }

        internal void ProfileRendered(SKCanvas canvas, SKRect bounds)
        {
            JsValue canvasValue = JsValue.FromObject(_pluginJintEngine.Engine, canvas);
            JsValue boundsValue = JsValue.FromObject(_pluginJintEngine.Engine, bounds);
            foreach (FunctionInstance callback in _renderedCallbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new[] {canvasValue, boundsValue});
                }
                catch (ExecutionCanceledException)
                {
                    _renderedCallbacks.Remove(callback);
                }
            }
        }
    }
}