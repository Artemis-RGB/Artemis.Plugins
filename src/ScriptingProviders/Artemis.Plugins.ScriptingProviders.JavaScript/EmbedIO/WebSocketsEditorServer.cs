using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.Plugins.ScriptingProviders.JavaScript.Services;
using EmbedIO.WebSockets;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.EmbedIO;

public class WebSocketsEditorServer : WebSocketModule
{
    private readonly ScriptEditorService _scriptEditorService;

    public WebSocketsEditorServer(string urlPath, ScriptEditorService scriptEditorService) : base(urlPath, true)
    {
        _scriptEditorService = scriptEditorService;
    }

    protected override async Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
    {
        await SendAsync(context, $"You sent me: {Encoding.GetString(buffer)}");
    }

    protected override async Task OnClientConnectedAsync(IWebSocketContext context)
    {
        await SendScript(context);
        await SendSuspended(context);
    }

    public async Task SetScript()
    {
        await SendCommand(new WebSocketCommand("reset"), null);
        await SendScript(null);
    }
    
    public async Task SetSuspended()
    {
        await SendSuspended(null);
    }

    public async Task SendCommand(WebSocketCommand command, IWebSocketContext? context)
    {
        if (context != null)
            await SendAsync(context, command.Serialize());
        else
            await BroadcastAsync(command.Serialize());
    }

    private async Task SendScript(IWebSocketContext? context)
    {
        IJavaScriptScript? script = _scriptEditorService.CurrentScript;
        if (script == null)
            return;

        string declarations = string.Join("\r\n", script.EngineManager.ScriptBindings.Select(s => s.GetDeclaration()));
        declarations += string.Join("\r\n", script.EngineManager.ContextBindings.Select(s => s.GetDeclaration()));
        declarations += "\r\n" + GenerateAssembliesDeclarations(script.EngineManager.ExtraAssemblies);

        await SendCommand(new WebSocketCommand("setDeclarations", declarations), context);
        if (script.ScriptConfiguration.PendingScriptContent != null)
            await SendCommand(new WebSocketCommand("setScript", script.ScriptConfiguration.PendingScriptContent), context);
    }
    
    private async Task SendSuspended(IWebSocketContext? context)
    {
        if (_scriptEditorService.Suspended)
            await SendCommand(new WebSocketCommand("suspend"), context);
        else
            await SendCommand(new WebSocketCommand("resume"), context);
    }

    private string GenerateAssembliesDeclarations(Dictionary<string, Assembly> extraAssemblies)
    {
        List<TypeScriptAssembly> typeScriptAssemblies = new();
        foreach ((string name, Assembly assembly) in extraAssemblies)
            typeScriptAssemblies.Add(new TypeScriptAssembly(name, assembly));

        return string.Join("\r\n", typeScriptAssemblies.Select(a => a.GenerateCode()));
    }
}