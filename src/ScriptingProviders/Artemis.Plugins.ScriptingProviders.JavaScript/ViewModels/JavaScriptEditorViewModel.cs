using System.Reactive.Disposables;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.EmbedIO;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.Services;
using Artemis.UI.Shared.ScriptingProviders;
using ReactiveUI;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;

public class JavaScriptEditorViewModel : ScriptEditorViewModel
{
    private readonly ScriptEditorService _scriptEditorService;
    public Plugin Plugin { get; }

    public JavaScriptEditorViewModel(ScriptEditorService scriptEditorService, ScriptType scriptType, Plugin plugin) : base(scriptType)
    {
        _scriptEditorService = scriptEditorService;
        Plugin = plugin;
        
        this.WhenActivated(d =>
        {
            _scriptEditorService.SetSuspended(false);
            Disposable.Create(() => _scriptEditorService.SetSuspended(true)).DisposeWith(d);
        });
    }

    protected override void OnScriptChanged(Script? script)
    {
        Task.Run(() => _scriptEditorService.SetScript(script as IJavaScriptScript));
    }
}