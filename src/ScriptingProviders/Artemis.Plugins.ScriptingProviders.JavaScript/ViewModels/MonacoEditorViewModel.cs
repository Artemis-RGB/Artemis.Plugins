using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Scripts;
using Artemis.UI.Shared;
using Artemis.UI.Shared.ScriptingProviders;
using Artemis.UI.Shared.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Ookii.Dialogs.Wpf;
using Stylet;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels
{
    public class MonacoEditorViewModel : ScriptEditorViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly Plugin _plugin;
        private string? _declarations;
        private WebView2 _editorWebView = null!;
        private bool _loadingEditor;
        private bool _subscribed;

        public MonacoEditorViewModel(ScriptType scriptType, Plugin plugin, IDialogService dialogService) : base(scriptType)
        {
            _plugin = plugin;
            _dialogService = dialogService;
            _loadingEditor = true;

            EditorUri = new Uri(plugin.Directory.FullName + "\\Editor\\index.html");
        }

        public IJavaScriptScript? JSScript { get; private set; }
        public Uri EditorUri { get; }

        public bool LoadingEditor
        {
            set => SetAndNotify(ref _loadingEditor, value);
            get => _loadingEditor;
        }

        public async Task ExportScript()
        {
            if (JSScript == null)
                return;

            VistaSaveFileDialog dialog = new()
            {
                Filter = "JavaScript file|*.js",
                Title = "Export Artemis Script"
            };
            bool? result = dialog.ShowDialog();
            if (result != true)
                return;

            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            string path = Path.ChangeExtension(dialog.FileName, ".js");
            await File.WriteAllTextAsync(path, currentScript);
        }

        public async Task ImportScript()
        {
            if (JSScript == null)
                return;

            VistaOpenFileDialog dialog = new()
            {
                Filter = "JavaScript file|*.js",
                Title = "Import Artemis Script"
            };
            bool? result = dialog.ShowDialog();
            if (result != true)
                return;

            string code = HttpUtility.JavaScriptStringEncode(await File.ReadAllTextAsync(dialog.FileName));

            // Use executeEdits to retain the undo/redo stack
            await _editorWebView.ExecuteScriptAsync("window.scriptEditor.executeEdits(null, [{\r\n" +
                                                    $"  text: \"{code}\",\r\n" +
                                                    "  range: window.scriptEditor.getModel().getFullModelRange()\r\n}" +
                                                    "]);\r\n" +
                                                    "window.scriptEditor.setSelection(new monaco.Selection(0, 0, 0, 0));");
        }

        public async Task TriggerEditorAction(string action)
        {
            _editorWebView.Focus();
            await _editorWebView.ExecuteScriptAsync($"window.scriptEditor.trigger(\"viewModel\", \"{action}\");");
        }

        public void OpenUrl(string url)
        {
            Utilities.OpenUrl(url);
        }

        public async Task UpdateScript(IJavaScriptScript? javaScriptScript)
        {
            JSScript = javaScriptScript;

            if (JSScript == null)
                return;

            // Load the default script
            if (string.IsNullOrWhiteSpace(JSScript.ScriptConfiguration.PendingScriptContent))
                JSScript.ScriptConfiguration.PendingScriptContent = ScriptType switch
                {
                    ScriptType.Global => await File.ReadAllTextAsync(_plugin.ResolveRelativePath("Templates/GlobalScript.js")),
                    ScriptType.Profile => await File.ReadAllTextAsync(_plugin.ResolveRelativePath("Templates/ProfileScript.js")),
                    _ => JSScript.ScriptConfiguration.PendingScriptContent
                };

            _declarations = string.Join("\r\n", JSScript.EngineManager.ScriptBindings.Select(s => s.GetDeclaration()));
            _declarations += string.Join("\r\n", JSScript.EngineManager.ContextBindings.Select(s => s.GetDeclaration()));
            _declarations += "\r\n" + GenerateAssembliesDeclarations(JSScript.EngineManager.ExtraAssemblies);

            if (!LoadingEditor)
                await ApplyScript();
        }

        public async void EditorWebViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                await UpdateScriptContents();
        }

        #region Overrides of ScriptEditorViewModel

        /// <inheritdoc />
        protected override void OnScriptChanged(Script? script)
        {
            Execute.PostToUIThread(async () => await UpdateScript(script as IJavaScriptScript));
        }

        #endregion

        private async Task UpdateScriptContents()
        {
            if (JSScript == null)
                return;

            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            JSScript.ScriptConfiguration.PendingScriptContent = currentScript;
            JSScript.ScriptConfiguration.ApplyPendingChanges();
        }

        private async Task UpdateHasChanges()
        {
            if (JSScript == null)
                return;

            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            JSScript.ScriptConfiguration.PendingScriptContent = currentScript;
        }

        private async void EditorWebViewOnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            await UpdateHasChanges();
        }

        private async void EditorWebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await ApplyScript();
            LoadingEditor = false;
        }

        private string GenerateAssembliesDeclarations(Dictionary<string, Assembly> extraAssemblies)
        {
            List<TypeScriptAssembly> typeScriptAssemblies = new();
            foreach ((string name, Assembly assembly) in extraAssemblies)
                typeScriptAssemblies.Add(new TypeScriptAssembly(name, assembly));

            return string.Join("\r\n", typeScriptAssemblies.Select(a => a.GenerateCode()));
        }

        private string GenerateExtraTypeDeclarations(List<Type> extraTypes)
        {
            return string.Join("\r\n", extraTypes.Select(v => new TypeScriptClass(null, v, true, TypeScriptClass.MaxDepth).GenerateCode("declare")));
        }

        private async Task ApplyScript()
        {
            if (JSScript == null)
                return;

            await _editorWebView.ExecuteScriptAsync(
                "if (window.dataModelDeclarations) window.dataModelDeclarations.dispose(); \r\n" +
                "if (window.assembliesDeclarations) window.assembliesDeclarations.dispose(); \r\n" +
                "if (window.extraDeclarations) window.extraDeclarations.dispose(); \r\n" +
                $"window.dataModelDeclarations = monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(_declarations)}\", 'artemisDeclarations.d.ts'); \r\n" +
                $"window.scriptEditor.setValue(\"{HttpUtility.JavaScriptStringEncode(JSScript.ScriptConfiguration.PendingScriptContent)}\");"
            );
        }

        #region Overrides of Screen

        /// <inheritdoc />
        public override async Task<bool> CanCloseAsync()
        {
            if (JSScript == null || !JSScript.ScriptConfiguration.HasChanges)
                return true;

            bool discard = await _dialogService.ShowConfirmDialogAt(
                "ScriptsDialog",
                JSScript.ScriptConfiguration.Name,
                "You have unsaved changes, do you want to discard them?",
                "Discard"
            );
            if (discard)
                JSScript.ScriptConfiguration.DiscardPendingChanges();

            return discard;
        }

        protected override void OnViewLoaded()
        {
            _editorWebView = VisualTreeUtilities.FindChild<WebView2>(View, "EditorWebView")!;
            if (_editorWebView == null)
                throw new ArtemisPluginException("Failed to find the editor");

            if (!_subscribed)
            {
                _editorWebView.NavigationCompleted += EditorWebViewOnNavigationCompleted;
                _editorWebView.WebMessageReceived += EditorWebViewOnWebMessageReceived;
                _subscribed = true;
            }

            _editorWebView.Unloaded += EditorWebViewOnUnloaded;

            base.OnViewLoaded();
        }

        private void EditorWebViewOnUnloaded(object sender, RoutedEventArgs e)
        {
            _editorWebView.NavigationCompleted -= EditorWebViewOnNavigationCompleted;
            _editorWebView.WebMessageReceived -= EditorWebViewOnWebMessageReceived;
            _subscribed = false;
        }

        #endregion
    }
}