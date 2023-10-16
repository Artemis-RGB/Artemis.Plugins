using System;
using System.IO;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;

namespace Artemis.Plugins.ScriptingProviders.JavaScript;

[PluginFeature(Name = "JavaScript Scripting Provider", AlwaysEnabled = true)]
public class JavaScriptScriptingProvider : ScriptingProvider<JavaScriptGlobalScript, JavaScriptProfileScript>
{
    private readonly Func<ScriptType, JavaScriptEditorViewModel> _createEditor;
    private readonly ScriptEditorService _scriptEditorService;
    private readonly Plugin _plugin;

    public JavaScriptScriptingProvider(Func<ScriptType, JavaScriptEditorViewModel> createEditor, ScriptEditorService scriptEditorService, Plugin plugin)
    {
        _createEditor = createEditor;
        _scriptEditorService = scriptEditorService;
        _plugin = plugin;
    }

    public override string LanguageName => "JavaScript";

    public override void Enable()
    {
        _scriptEditorService.Initialize(this);
    }

    public override void Disable()
    {
    }

    public override IScriptEditorViewModel CreateScriptEditor(ScriptType scriptType)
    {
        return _createEditor(scriptType);
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