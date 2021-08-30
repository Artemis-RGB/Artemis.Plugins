using Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public interface IContextBinding
    {
        void Initialize(Engine engine);
        string? GetDeclaration();
    }
}