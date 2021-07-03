using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript
{
    [PluginFeature(Name = "JavaScript Scripting Provider", Icon = "LanguageJavascript")]
    public class JavaScriptScriptingProvider : ScriptingProvider<JavaScriptGlobalScript, JavaScriptProfileScript>
    {
        private readonly IKernel _kernel;

        public JavaScriptScriptingProvider(IKernel kernel)
        {
            _kernel = kernel;
        }
        
        public override string LanguageName => "JavaScript";

        public override void Enable()
        {
        }

        public override void Disable()
        {
        }

        public override IScriptEditorViewModel CreateScriptEditor(ScriptType scriptType)
        {
            return _kernel.Get<MonacoEditorViewModel>(new ConstructorArgument("scriptType", scriptType));
        }
    }
}