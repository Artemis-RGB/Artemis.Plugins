using System;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Runtime.Interop;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class TimeSpanBinding : IInstanceBinding
    {
        public void Initialize(EngineManager engineManager)
        {
            engineManager.Engine!.SetValue("TimeSpan", TypeReference.CreateTypeReference(engineManager.Engine, typeof(TimeSpan)));
        }

        public string GetDeclaration()
        {
            return new TypeScriptClass(null, typeof(TimeSpan), true, TypeScriptClass.MaxDepth).GenerateCode("declare");
        }
    }
}