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
    public class ProfileBinding : IManualScriptBinding
    {
        private readonly Plugin _plugin;
        private readonly PluginJintEngine _pluginJintEngine;
        private readonly Profile _profile;
        private readonly List<FunctionInstance> _renderedCallbacks = new();
        private readonly List<FunctionInstance> _renderingCallbacks = new();
        private readonly List<FunctionInstance> _updatedCallbacks = new();

        private readonly List<FunctionInstance> _updatingCallbacks = new();

        public ProfileBinding(Profile profile, Plugin plugin, PluginJintEngine pluginJintEngine)
        {
            _profile = profile;
            _plugin = plugin;
            _pluginJintEngine = pluginJintEngine;
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
            if (_pluginJintEngine.Engine == null)
                return;

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
            if (_pluginJintEngine.Engine == null)
                return;

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

        #region Implementation of IManualScriptBinding

        /// <inheritdoc />
        public string Declaration => File.ReadAllText(_plugin.ResolveRelativePath("StaticDeclarations/ProfileBinding.ts"));

        #endregion

        #region JS functions

        // ReSharper disable InconsistentNaming
        public Action onUpdating(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatingCallbacks.Remove(functionInstance);
        }


        public Action onUpdated(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _updatedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _updatedCallbacks.Remove(functionInstance);
        }


        public Action onRendering(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderingCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderingCallbacks.Remove(functionInstance);
        }


        public Action onRendered(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _renderedCallbacks.Add(callback.As<FunctionInstance>());

            return () => _renderedCallbacks.Remove(functionInstance);
        }


        public Folder[] getFolders()

        {
            return _profile.GetAllFolders().ToArray();
        }

        public Layer[] getLayers()
        {
            return _profile.GetAllLayers().ToArray();
        }
        // ReSharper restore InconsistentNaming

        #endregion
    }

    public interface IManualScriptBinding
    {
        public string Declaration { get; }
    }
}