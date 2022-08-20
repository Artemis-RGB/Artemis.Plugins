import type { WebSocketCommand } from "@/models/WebSocketCommand";
import type { EditorWebSocketCommandHandler } from "./EditorWebSocketCommandHandler";

export class EditorWebSocket {
    public connection: WebSocket | undefined;
    public commandHandlers: EditorWebSocketCommandHandler[] = [];

    constructor(private state: { suspended: boolean }) {
        this.connect();
    }

    public sendCommand(command: WebSocketCommand) {
        if (this.connection) {
            this.connection.send(JSON.stringify(command));
        }
    }

    private connect() {
        this.connection = new WebSocket("ws://localhost:9696/dd35f1b7-3d3f-4f90-a60f-40354783049b/editor-ws");
        this.connection.onclose = _ => setTimeout(() => {
            this.state.suspended = true;
            this.connect();
        }, 250);
        this.connection.onmessage = m => this.handleMessage(m);
    }

    private handleMessage(m: MessageEvent<any>): any {            
        const command = JSON.parse(m.data) as WebSocketCommand
        for (const commandHandler of this.commandHandlers) {
            if (commandHandler.command == command.Command) {
                commandHandler.handler(command.Argument);
            }
        }
    }
}
