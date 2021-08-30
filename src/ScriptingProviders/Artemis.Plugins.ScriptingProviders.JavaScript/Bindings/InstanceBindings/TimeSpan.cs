using System;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Jint;
using Jint.Runtime.Interop;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class TimeSpanBinding : IInstanceBinding
    {
        public void Initialize(Engine engine)
        {
            engine.SetValue("TimeSpan", TypeReference.CreateTypeReference(engine, typeof(TimeSpan)));
        }

        public string GetDeclaration()
        {
            return new TypeScriptClass(null, typeof(TimeSpan), true, TypeScriptClass.MaxDepth).GenerateCode("declare");
        }
    }
}