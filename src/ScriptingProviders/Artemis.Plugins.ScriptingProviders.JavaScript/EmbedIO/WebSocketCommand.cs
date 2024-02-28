using System.Text.Json;
using System.Text.Json.Serialization;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.EmbedIO;

public class WebSocketCommand
{
    public WebSocketCommand(string command)
    {
        Command = command;
    }

    [JsonConstructor]
    public WebSocketCommand(string command, string argument)
    {
        Command = command;
        Argument = argument;
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public string Command { get; }
    public string? Argument { get; }
}