<template>
  <h1 v-show="state.suspended">Open the script editor in Artemis to start editing here :)</h1>
  <div v-show="!state.suspended" id="editor" ref="editor"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from "vue";
import loader from "@monaco-editor/loader";
import { EditorWebSocket } from "@/websockets/EditorWebSocket";
import { EditorWebSocketCommandHandler } from "@/websockets/EditorWebSocketCommandHandler";
import type { editor as standAloneEditor, IDisposable } from "monaco-editor";

const state = reactive({ suspended: false })
const editor = ref<HTMLInputElement | null>(null);
let monacoEditor: standAloneEditor.IStandaloneCodeEditor | undefined;
let declarations: IDisposable | null = null;


onMounted(() => {
  loader.init().then((monaco) => {
    const editorWebSocket = new EditorWebSocket();
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("reset", () => monacoEditor?.setValue("")));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("setScript", (a) => monacoEditor?.setValue(a ?? "")));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("suspend", (a) => state.suspended = true));
    editorWebSocket.commandHandlers.push(new EditorWebSocketCommandHandler("resume", (a) => state.suspended = false));
    editorWebSocket.commandHandlers.push(
      new EditorWebSocketCommandHandler("setDeclarations", (a) => {
        if (a) {
          if (declarations) {
            declarations.dispose();
          }
          declarations =
            monaco.languages.typescript.javascriptDefaults.addExtraLib(
              a,
              "artemisDeclarations.d.ts"
            );
        }
      })
    );

    // validation settings
    monaco.languages.typescript.javascriptDefaults.setDiagnosticsOptions({
      noSemanticValidation: false,
      noSyntaxValidation: false,
    });

    // compiler options
    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
      target: monaco.languages.typescript.ScriptTarget.ES2020,
      allowNonTsExtensions: true,
    });

    if (editor.value) {
      monacoEditor = monaco.editor.create(editor.value, {
        language: "javascript",
        lineNumbers: "on",
        readOnly: false,
        automaticLayout: true,
        theme: "vs-dark",
      });
    }
  });
});
</script>

<style>
#editor {
  height: 100vh;
  overflow: hidden;
  width: 100vw;
}

body {
  margin: 0;
}
</style>