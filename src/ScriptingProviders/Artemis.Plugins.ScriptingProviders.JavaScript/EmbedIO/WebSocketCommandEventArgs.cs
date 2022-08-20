using System;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.EmbedIO;

public class WebSocketCommandEventArgs : EventArgs
{
    public WebSocketCommand Command { get; }

    public WebSocketCommandEventArgs(WebSocketCommand command)
    {
        Command = command;
    }
}