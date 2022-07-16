<template>
  <div id="suspended-container" v-show="state.suspended">
    <img src="../../public/favicon.ico" width="80" height="80">
    <h2>Open the script editor in Artemis to start editing here :)</h2>
  </div>
  <div v-show="!state.suspended" id="editor" ref="editor"></div>
</template>

<script setup lang="ts">
import {ref, onMounted, reactive} from "vue";
import loader from "@monaco-editor/loader";
import {EditorWebSocket} from "@/websockets/EditorWebSocket";
import {EditorWebSocketCommandHandler} from "@/websockets/EditorWebSocketCommandHandler";
import type {IDisposable} from "monaco-editor";

const state = reactive({suspended: true})
const editor = ref<HTMLInputElement | null>(null);

onMounted(() => {
  loader.init().then((monaco) => {
    if (!editor.value) {
      return;
    }

    const editorWebSocket = new EditorWebSocket(state);
    const monacoEditor = monaco.editor.create(editor.value, {
      language: "javascript",
      lineNumbers: "on",
      readOnly: false,
      automaticLayout: true,
      theme: "vs-dark"
    });

    let declarations: IDisposable | null = null;
    let hasSetScript = false;

    // validation settings
    monaco.languages.typescript.javascriptDefaults.setDiagnosticsOptions({
      noSemanticValidation: false,
      noSyntaxValidation: false
    });

    // compiler options
    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
      noLib: true,
      target: monaco.languages.typescript.ScriptTarget.ES2020,
      allowNonTsExtensions: true
    });

    // Script state handlers
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("reset", () => {
      hasSetScript = false;
      monacoEditor.setValue("");
      document.title = "Artemis JS Editor";
    }));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("setScript", a => {
      // setValue seems to be async, here's an ugly workaround to make sure we're ready
      monacoEditor.setValue(a ?? "");
      window.requestIdleCallback(() => hasSetScript = true);
    }));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("setTitle", a => {
      document.title = a ? `Artemis JS Editor | ${a}` : "Artemis JS Editor";
    }));

    // Suspension state handlers
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("suspend", () => {
      document.title = "Artemis JS Editor";
      state.suspended = true;
    }));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("resume", () => state.suspended = false));

    // Editor config handlers
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("setTheme", (a) => monaco.editor.setTheme(a ?? "vs-dark")));
    editorWebSocket.commandHandlers.push(
        new EditorWebSocketCommandHandler("setDeclarations", (a) => {
          if (a) {
            if (declarations) {
              declarations.dispose();
            }
            declarations = monaco.languages.typescript.javascriptDefaults.addExtraLib(a, "artemisDeclarations.d.ts");
          }
        })
    );

    monacoEditor.onDidChangeModelContent(() => {
      if (!hasSetScript) {
        return;
      }
      const value = monacoEditor.getValue();
      if (value) {
        editorWebSocket.sendCommand({Command: "valueChanged", Argument: value});
      }
    })

    monacoEditor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyS, () => editorWebSocket.sendCommand({
      Command: "save",
      Argument: null
    }))
  });
});
</script>

<style>
#suspended-container {
  font-family: -apple-system, BlinkMacSystemFont, "Segoe WPC", "Segoe UI", "HelveticaNeue-Light", system-ui, "Ubuntu", "Droid Sans", sans-serif;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

#editor {
  height: 100vh;
  overflow: hidden;
  width: 100vw;
}

body {
  margin: 0;
}
</style>