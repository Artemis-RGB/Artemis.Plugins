using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.ContextBindings
{
    public class ProfileBinding : IContextBinding
    {
        private readonly Plugin _plugin;
        private readonly Profile _profile;
        private readonly List<Function> _renderedCallbacks = new();
        private readonly List<Function> _renderingCallbacks = new();
        private readonly List<Function> _updatedCallbacks = new();
        private readonly List<Function> _updatingCallbacks = new();
        private Engine? _engine;

        public ProfileBinding(Profile profile, Plugin plugin)
        {
            _profile = profile;
            _plugin = plugin;
        }

        internal void ProfileUpdating(double deltaTime)
        {
            foreach (Function callback in _updatingCallbacks.ToList())
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
            foreach (Function callback in _updatedCallbacks.ToList())
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
            if (_engine == null)
                return;

            JsValue[] arguments =
            {
                JsValue.FromObject(_engine, canvas),
                JsValue.FromObject(_engine, bounds)
            };
            foreach (Function callback in _renderingCallbacks.ToList())
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

        internal void ProfileRendered(SKCanvas canvas, SKRect bounds)
        {
            if (_engine == null)
                return;

            JsValue[] arguments =
            {
                JsValue.FromObject(_engine, canvas),
                JsValue.FromObject(_engine, bounds)
            };
            foreach (Function callback in _renderedCallbacks.ToList())
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

        public void Initialize(EngineManager engineManager)
        {
            _engine = engineManager.Engine;
            if (_engine != null)
                _engine.SetValue("Profile", this);
        }

        public string GetDeclaration()
        {
            return File.ReadAllText(_plugin.ResolveRelativePath("StaticDeclarations/ProfileWrapper.ts"));
        }


        #region JS functions

        public Action? OnUpdating(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _updatingCallbacks.Add(function);
            return () => _updatingCallbacks.Remove(function);
        }

        public Action? OnUpdated(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _updatedCallbacks.Add(function);
            return () => _updatedCallbacks.Remove(function);
        }

        public Action? OnRendering(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _renderingCallbacks.Add(function);
            return () => _renderingCallbacks.Remove(function);
        }

        public Action? OnRendered(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _renderedCallbacks.Add(function);
            return () => _renderedCallbacks.Remove(function);
        }

        public Profile GetProfile()
        {
            return _profile;
        }

        public Folder[] GetFolders()

        {
            return _profile.GetAllFolders().ToArray();
        }

        public Layer[] GetLayers()
        {
            return _profile.GetAllLayers().ToArray();
        }

        #endregion
    }
}