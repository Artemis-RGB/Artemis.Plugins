using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public interface IJavaScriptScript
    {
        ScriptConfiguration ScriptConfiguration { get; }
        PluginJintEngine Engine { get; }
    }
}