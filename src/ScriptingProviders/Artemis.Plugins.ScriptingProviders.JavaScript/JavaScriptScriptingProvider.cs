using System;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript
{
    [PluginFeature(Name = "JavaScript Scripting Provider", Icon = "LanguageJavascript")]
    public class JavaScriptScriptingProvider : ScriptingProvider<JavaScriptGlobalScript, JavaScriptProfileScript, JavaScriptLayerScript, JavaScriptLayerPropertyScript>
    {
        private readonly IKernel _kernel;

        public JavaScriptScriptingProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void Enable()
        {
        }

        public override void Disable()
        {
        }

        public override IScriptEditorViewModel CreateGlobalScriptEditor(JavaScriptGlobalScript script)
        {
            return _kernel.Get<JavaScriptScriptEditorViewModel>(new ConstructorArgument("script", script));
        }

        public override IScriptEditorViewModel CreateProfileScriptEditor(JavaScriptProfileScript script)
        {
            return _kernel.Get<JavaScriptScriptEditorViewModel>(new ConstructorArgument("script", script));
        }

        public override IScriptEditorViewModel CreateLayerScriptScriptEditor(JavaScriptLayerScript script)
        {
            return _kernel.Get<JavaScriptScriptEditorViewModel>(new ConstructorArgument("script", script));
        }

        public override IScriptEditorViewModel CreatePropertyScriptEditor(JavaScriptLayerPropertyScript script)
        {
            return _kernel.Get<JavaScriptScriptEditorViewModel>(new ConstructorArgument("script", script));
        }
    }
}