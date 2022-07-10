using System;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.EmbedIO;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using EmbedIO.Files;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Services;

public class ScriptEditorService
{
    private const string EDITOR_URL = "dd35f1b7-3d3f-4f90-a60f-40354783049b/editor";
    private readonly Plugin _plugin;
    private readonly IWebServerService _webServerService;
    private WebSocketsEditorServer? _websocketModule;

    public ScriptEditorService(Plugin plugin, IWebServerService webServerService)
    {
        _plugin = plugin;
        _webServerService = webServerService;
    }

    public IJavaScriptScript? CurrentScript { get; private set; }
    public bool Suspended { get; private set; } = true;
    public string EditorUrl => $"{_webServerService.Server!.Listener.Prefixes.First().Replace("*", "localhost")}{EDITOR_URL}";

    public void Initialize(PluginFeature pluginFeature)
    {
        // Serve the static files of the editor web application
        _webServerService.AddModule(pluginFeature, () =>
        {
            FileSystemProvider provider = new(_plugin.ResolveRelativePath("WebApplication\\dist"), true);
            return new FileModule($"/{EDITOR_URL}", provider);
        });

        _webServerService.AddModule(pluginFeature, () =>
        {
            _websocketModule = new WebSocketsEditorServer($"/{EDITOR_URL}-ws", this);
            _websocketModule.WebSocketCommandReceived += (sender, args) => WebSocketCommandReceived?.Invoke(sender, args);
            return _websocketModule;
        });
    }

    public async Task SetScript(IJavaScriptScript? script)
    {
        if (CurrentScript == script)
            return;

        CurrentScript = script;
        if (_websocketModule != null)
            await _websocketModule.SetScript();
    }

    public async Task SendCommand(WebSocketCommand command)
    {
        if (_websocketModule != null)
            await _websocketModule.SendCommand(command, null);
    }

    public void SetSuspended(bool suspended)
    {
        Suspended = suspended;
        if (_websocketModule != null)
            Task.Run(async () => await _websocketModule.SetSuspended());
    }

    public event EventHandler<WebSocketCommandEventArgs>? WebSocketCommandReceived;
}