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
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual;
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
        private readonly IDataModelService _dataModelService;
        private readonly IDialogService _dialogService;
        private readonly Plugin _plugin;
        private string? _assembliesDeclarations;
        private string? _dataModelDeclarations;
        private WebView2 _editorWebView = null!;
        private string? _extraDeclarations;
        private bool _loadingEditor;
        private bool _subscribed;

        public MonacoEditorViewModel(ScriptType scriptType, Plugin plugin, IDialogService dialogService, IDataModelService dataModelService) : base(scriptType)
        {
            _plugin = plugin;
            _dialogService = dialogService;
            _dataModelService = dataModelService;
            _loadingEditor = true;

            EditorUri = new Uri(plugin.Directory.FullName + "\\Editor\\index.html");
        }

        public IJavaScriptScript? JavaScriptScript { get; private set; }
        public Uri EditorUri { get; }

        public bool LoadingEditor
        {
            set => SetAndNotify(ref _loadingEditor, value);
            get => _loadingEditor;
        }

        public async Task ExportScript()
        {
            if (JavaScriptScript == null)
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
            if (JavaScriptScript == null)
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
            Core.Utilities.OpenUrl(url);
        }

        public async Task UpdateScript(IJavaScriptScript? javaScriptScript)
        {
            JavaScriptScript = javaScriptScript;

            if (javaScriptScript == null)
                return;

            // Load the default script
            if (string.IsNullOrWhiteSpace(javaScriptScript.ScriptConfiguration.PendingScriptContent))
            {
                javaScriptScript.ScriptConfiguration.PendingScriptContent = ScriptType switch
                {
                    ScriptType.Global => await File.ReadAllTextAsync(_plugin.ResolveRelativePath("Templates/GlobalScript.js")),
                    ScriptType.Profile => await File.ReadAllTextAsync(_plugin.ResolveRelativePath("Templates/ProfileScript.js")),
                    _ => javaScriptScript.ScriptConfiguration.PendingScriptContent
                };
            }

            _dataModelDeclarations = GenerateDataModelDeclarations();
            _assembliesDeclarations = GenerateAssembliesDeclarations(javaScriptScript.Engine.ExtraAssemblies);
            _extraDeclarations = $"{GenerateExtraDeclarations(javaScriptScript.Engine.ExtraValues)}\r\n" +
                                 $"{GenerateExtraTypeDeclarations(javaScriptScript.Engine.ExtraTypes)}";

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
            if (JavaScriptScript == null)
                return;

            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            JavaScriptScript.ScriptConfiguration.PendingScriptContent = currentScript;
            JavaScriptScript.ScriptConfiguration.ApplyPendingChanges();
        }

        private async Task UpdateHasChanges()
        {
            if (JavaScriptScript == null)
                return;

            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            JavaScriptScript.ScriptConfiguration.PendingScriptContent = currentScript;
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

        private string GenerateDataModelDeclarations()
        {
            TypeScriptDataModelCollection dataModelCollection = new(_dataModelService.GetDataModels());
            return dataModelCollection.GenerateCode();
        }

        private string GenerateAssembliesDeclarations(Dictionary<string, Assembly> extraAssemblies)
        {
            List<TypeScriptAssembly> typeScriptAssemblies = new();
            foreach ((string name, Assembly assembly) in extraAssemblies)
                typeScriptAssemblies.Add(new TypeScriptAssembly(name, assembly));

            return string.Join("\r\n", typeScriptAssemblies.Select(a => a.GenerateCode()));
        }

        private string GenerateExtraDeclarations(Dictionary<string, IManualScriptBinding> manualScriptBindings)
        {
            return string.Join("\r\n", manualScriptBindings.Values.Select(v => v.Declaration));
        }

        private string GenerateExtraTypeDeclarations(List<Type> extraTypes)
        {
            return string.Join("\r\n", extraTypes.Select(v => new TypeScriptClass(null, v, true, TypeScriptClass.MaxDepth).GenerateCode("declare")));
        }

        private async Task ApplyScript()
        {
            if (JavaScriptScript == null)
                return;

            await _editorWebView.ExecuteScriptAsync(
                "if (window.dataModelDeclarations) window.dataModelDeclarations.dispose(); \r\n" +
                "if (window.assembliesDeclarations) window.assembliesDeclarations.dispose(); \r\n" +
                "if (window.extraDeclarations) window.extraDeclarations.dispose(); \r\n" +
                $"window.dataModelDeclarations = monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(_dataModelDeclarations)}\", 'dataModelDeclarations.d.ts'); \r\n" +
                $"window.assembliesDeclarations = monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(_assembliesDeclarations)}\", 'assembliesDeclarations.d.ts'); \r\n" +
                $"window.extraDeclarations = monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(_extraDeclarations)}\", 'extraDeclarations.d.ts'); \r\n" +
                $"window.scriptEditor.setValue(\"{HttpUtility.JavaScriptStringEncode(JavaScriptScript.ScriptConfiguration.PendingScriptContent)}\");"
            );
        }

        #region Overrides of Screen

        /// <inheritdoc />
        public override async Task<bool> CanCloseAsync()
        {
            if (JavaScriptScript == null || !JavaScriptScript.ScriptConfiguration.HasChanges)
                return true;

            bool discard = await _dialogService.ShowConfirmDialogAt(
                "ScriptsDialog",
                JavaScriptScript.ScriptConfiguration.Name,
                "You have unsaved changes, do you want to discard them?",
                "Discard"
            );
            if (discard)
                JavaScriptScript.ScriptConfiguration.DiscardPendingChanges();

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