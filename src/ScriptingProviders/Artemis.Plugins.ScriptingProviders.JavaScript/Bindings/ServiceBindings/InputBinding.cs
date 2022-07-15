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
        private readonly List<FunctionInstance> _keyDownCallbacks = new();
        private readonly List<FunctionInstance> _keyUpCallbacks = new();
        private readonly List<FunctionInstance> _mouseDownCallbacks = new();
        private readonly List<FunctionInstance> _mouseUpCallbacks = new();
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

        public Action OnKeyDown(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _keyDownCallbacks.Add(callback.As<FunctionInstance>());

            return () => _keyDownCallbacks.Remove(functionInstance);
        }

        public Action OnKeyUp(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _keyUpCallbacks.Add(callback.As<FunctionInstance>());

            return () => _keyUpCallbacks.Remove(functionInstance);
        }

        public Action OnMouseDown(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _mouseDownCallbacks.Add(callback.As<FunctionInstance>());

            return () => _mouseDownCallbacks.Remove(functionInstance);
        }

        public Action OnMouseUp(JsValue callback)
        {
            FunctionInstance functionInstance = callback.As<FunctionInstance>();
            _mouseUpCallbacks.Add(callback.As<FunctionInstance>());

            return () => _mouseUpCallbacks.Remove(functionInstance);
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
            List<FunctionInstance> callbacks = e.IsDown ? _keyDownCallbacks : _keyUpCallbacks;
            foreach (FunctionInstance callback in callbacks.ToList())
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
            List<FunctionInstance> callbacks = e.IsDown ? _mouseDownCallbacks : _mouseUpCallbacks;
            foreach (FunctionInstance callback in callbacks.ToList())
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