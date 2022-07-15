using System;
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
    
    public JavaScriptEditorViewModel(ScriptEditorService scriptEditorService, ScriptType scriptType, Plugin plugin) : base(scriptType)
    {
        _scriptEditorService = scriptEditorService;
        Plugin = plugin;
        EditorUrl = scriptEditorService.EditorUrl;
        WebViewSupported = OperatingSystem.IsMacOS() || OperatingSystem.IsWindows();
        
        this.WhenActivated(d =>
        {
            _scriptEditorService.SetSuspended(false);
            _scriptEditorService.WebSocketCommandReceived += ScriptEditorServiceOnWebSocketCommandReceived;
            Disposable.Create(() =>
            {
                _scriptEditorService.WebSocketCommandReceived -= ScriptEditorServiceOnWebSocketCommandReceived;
                _scriptEditorService.SetSuspended(true);
            }).DisposeWith(d);
        });
    }
    
    public Plugin Plugin { get; }
    public string EditorUrl { get; }
    public bool WebViewSupported { get; }
    
    private void ScriptEditorServiceOnWebSocketCommandReceived(object? sender, WebSocketCommandEventArgs e)
    {
        if (Script == null)
            return;
        
        if (e.Command.Command == "valueChanged")
            Script.ScriptConfiguration.PendingScriptContent = e.Command.Argument;
        else if (e.Command.Command == "save")
            Script.ScriptConfiguration.ApplyPendingChanges();
    }

    protected override void OnScriptChanged(Script? script)
    {
        Task.Run(() => _scriptEditorService.SetScript(script as IJavaScriptScript));
    }
}