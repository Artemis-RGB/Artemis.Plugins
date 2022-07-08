export class EditorWebSocketCommandHandler {
    constructor(public command: string, public handler: (params: string | null) => void) {}
}