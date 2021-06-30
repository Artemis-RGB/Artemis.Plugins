using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public class EasingBinding : IScriptBinding
    {
        public EasingBinding(PluginJintEngine engine)
        {
            TypeScriptEnum typeScriptEnum = new(typeof(Easings.Functions));
            string enums = "";
            for (int index = 0; index < typeScriptEnum.Names.Length; index++)
                enums = enums + typeScriptEnum.Names[index] + ": " + typeScriptEnum.Values[index] + ",\r\n";

            string enumDeclaration = "const Artemis = {\r\n" +
                                     "    Core: {}\r\n" +
                                     "}\r\n" +
                                     "Artemis.Core.EasingsFunctions = {\r\n" +
                                     $"   {enums.Trim()}\r\n" +
                                     "}";

            engine.Engine.Execute(enumDeclaration);
        }


        public string? Name => null;
    }
}