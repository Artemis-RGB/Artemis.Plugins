using System.IO;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript;

[PluginFeature(Name = "JavaScript Scripting Provider", AlwaysEnabled = true)]
public class JavaScriptScriptingProvider : ScriptingProvider<JavaScriptGlobalScript, JavaScriptProfileScript>
{
    private readonly IKernel _kernel;
    private readonly Plugin _plugin;

    public JavaScriptScriptingProvider(IKernel kernel, ScriptEditorService scriptEditorService, Plugin plugin)
    {
        _kernel = kernel;
        _plugin = plugin;
        scriptEditorService.Initialize(this);
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
        return _kernel.Get<JavaScriptEditorViewModel>(new ConstructorArgument("scriptType", scriptType));
    }

    public override string GetDefaultScriptContent(ScriptType scriptType)
    {
        return scriptType switch
        {
            ScriptType.Profile => File.ReadAllText(_plugin.ResolveRelativePath("Templates/ProfileScript.js")),
            ScriptType.Global => File.ReadAllText(_plugin.ResolveRelativePath("Templates/GlobalScript.js")),
            _ => ""
        };
    }
}