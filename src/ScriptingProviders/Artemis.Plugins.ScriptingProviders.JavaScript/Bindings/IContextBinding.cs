using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public interface IContextBinding
    {
        void Initialize(EngineManager engineManager);
        string? GetDeclaration();
    }
}