using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.ServiceBindings
{
    public class InputBinding : IServiceBinding
    {
        private readonly IInputService _inputService;
        private readonly List<Function> _keyDownCallbacks = new();
        private readonly List<Function> _keyUpCallbacks = new();
        private readonly List<Function> _mouseDownCallbacks = new();
        private readonly List<Function> _mouseUpCallbacks = new();
        private readonly Plugin _plugin;

        public InputBinding(Plugin plugin, IInputService inputService)
        {
            _plugin = plugin;
            _inputService = inputService;
            _inputService.KeyboardKeyUpDown += InputServiceOnKeyboardKeyUpDown;
            _inputService.MouseButtonUpDown += InputServiceOnMouseButtonUpDown;
        }

        public bool IsKeyDown(KeyboardKey key)
        {
            return _inputService.IsKeyDown(key);
        }

        public bool IsButtonDown(MouseButton button)
        {
            return _inputService.IsButtonDown(button);
        }

        public Action? OnKeyDown(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _keyDownCallbacks.Add(function);
            return () => _keyDownCallbacks.Remove(function);
        }

        public Action? OnKeyUp(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _keyUpCallbacks.Add(function);
            return () => _keyUpCallbacks.Remove(function);
        }

        public Action? OnMouseDown(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _mouseDownCallbacks.Add(function);
            return () => _mouseDownCallbacks.Remove(function);
        }

        public Action? OnMouseUp(JsValue callback)
        {
            Function? function = callback.As<Function>();
            if (function == null)
                return null;
            
            _mouseUpCallbacks.Add(function);
            return () => _mouseUpCallbacks.Remove(function);
        }

        private void DeclareEnums(Engine engine, Type type, string name)
        {
            TypeScriptEnum typeScriptEnum = new(type);
            string enums = "";
            for (int index = 0; index < typeScriptEnum.Names.Length; index++)
                enums = $"{enums}{typeScriptEnum.Names[index]}: {typeScriptEnum.Values[index]},\r\n";

            string enumDeclaration = "if (!Artemis.Core) {\r\n" +
                                     "  Artemis.Core = {}\r\n" +
                                     "}\r\n" +
                                     $"{name} = {{\r\n" +
                                     $"   {enums.Trim()}\r\n" +
                                     "};";

            engine.Execute(enumDeclaration);
        }

        private void InputServiceOnKeyboardKeyUpDown(object? sender, ArtemisKeyboardKeyUpDownEventArgs e)
        {
            List<Function> callbacks = e.IsDown ? _keyDownCallbacks : _keyUpCallbacks;
            foreach (Function callback in callbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new JsValue[] {new JsNumber((int) e.Key)});
                }
                catch (ExecutionCanceledException)
                {
                    callbacks.Remove(callback);
                }
            }
        }

        private void InputServiceOnMouseButtonUpDown(object? sender, ArtemisMouseButtonUpDownEventArgs e)
        {
            List<Function> callbacks = e.IsDown ? _mouseDownCallbacks : _mouseUpCallbacks;
            foreach (Function callback in callbacks.ToList())
            {
                try
                {
                    callback.Call(JsValue.Undefined, new JsValue[] {new JsNumber((int) e.Button)});
                }
                catch (ExecutionCanceledException)
                {
                    callbacks.Remove(callback);
                }
            }
        }

        public void Initialize(EngineManager engineManager)
        {
            DeclareEnums(engineManager.Engine!, typeof(KeyboardKey), "Artemis.Core.KeyboardKey");
            DeclareEnums(engineManager.Engine!, typeof(MouseButton), "Artemis.Core.MouseButton");
            engineManager.Engine!.SetValue("Input", this);
        }

        public string GetDeclaration()
        {
            return File.ReadAllText(_plugin.ResolveRelativePath("StaticDeclarations/InputWrapper.ts"));
        }
    }
}