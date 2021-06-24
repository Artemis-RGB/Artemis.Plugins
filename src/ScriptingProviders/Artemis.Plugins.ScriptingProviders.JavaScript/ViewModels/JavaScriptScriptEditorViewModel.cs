using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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

namespace Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels
{
    public class JavaScriptScriptEditorViewModel : ScriptEditorViewModel
    {
        private readonly IDataModelService _dataModelService;
        private readonly IDialogService _dialogService;
        private WebView2 _editorWebView = null!;
        private bool _loadingEditor;

        public JavaScriptScriptEditorViewModel(Plugin plugin, Script script, IDialogService dialogService, IDataModelService dataModelService) : base(script)
        {
            if (script is not IJavaScriptScript javaScriptScript)
                throw new ArtemisPluginException($"Expected script to be an {nameof(IJavaScriptScript)}");

            _dialogService = dialogService;
            _dataModelService = dataModelService;
            _loadingEditor = true;

            EditorUri = new Uri(plugin.Directory.FullName + "\\Editor\\index.html");

            DataModelDeclarations = GenerateDataModelJavaScript();
            AssembliesDeclarations = GenerateAssembliesDeclarations(javaScriptScript.Engine.ExtraAssemblies);
            ExtraDeclarations = GenerateExtraDeclarations(javaScriptScript.Engine.ExtraValues) +
                                "\r\n" +
                                GenerateExtraTypeDeclarations(javaScriptScript.Engine.ExtraTypes);
        }

        public Uri EditorUri { get; }
        public string DataModelDeclarations { get; }
        public string AssembliesDeclarations { get; }
        public string ExtraDeclarations { get; }

        public bool LoadingEditor
        {
            set => SetAndNotify(ref _loadingEditor, value);
            get => _loadingEditor;
        }

        public async void EditorWebViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                await UpdateScriptContents();
        }

        private async Task UpdateScriptContents()
        {
            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            Script.ScriptConfiguration.PendingScriptContent = currentScript;
            Script.ScriptConfiguration.ApplyPendingChanges();
        }

        private async Task UpdateHasChanges()
        {
            string value = await _editorWebView.ExecuteScriptAsync("window.scriptEditor.getValue()");
            string currentScript = Regex.Unescape(value);
            currentScript = currentScript.Substring(1, currentScript.Length - 2);

            Script.ScriptConfiguration.PendingScriptContent = currentScript;
        }

        private async void EditorWebViewOnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            await UpdateHasChanges();
        }

        private async void EditorWebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await _editorWebView.ExecuteScriptAsync(
                $"monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(DataModelDeclarations)}\", 'dataModelDeclarations.d.ts'); \r\n" +
                $"monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(AssembliesDeclarations)}\", 'assembliesDeclarations.d.ts'); \r\n" +
                $"monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(ExtraDeclarations)}\", 'extraDeclarations.d.ts'); \r\n" +
                $"window.scriptEditor.setValue(\"{HttpUtility.JavaScriptStringEncode(Script.ScriptConfiguration.PendingScriptContent)}\");"
            );
            LoadingEditor = false;
        }

        private string GenerateDataModelJavaScript()
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

        #region Overrides of Screen

        /// <inheritdoc />
        public override async Task<bool> CanCloseAsync()
        {
            if (!Script.ScriptConfiguration.HasChanges)
                return true;

            bool discard = await _dialogService.ShowConfirmDialogAt("ScriptsDialog", "Unsaved changes", "You have unsaved changes, do you want to discard them?");
            if (discard)
                Script.ScriptConfiguration.DiscardPendingChanges();

            return discard;
        }

        protected override void OnViewLoaded()
        {
            _editorWebView = VisualTreeUtilities.FindChild<WebView2>(View, "EditorWebView")!;
            if (_editorWebView == null)
                throw new ArtemisPluginException("Failed to find the editor");

            _editorWebView.NavigationCompleted += EditorWebViewOnNavigationCompleted;
            _editorWebView.WebMessageReceived += EditorWebViewOnWebMessageReceived;

            base.OnViewLoaded();
        }

        #endregion
    }
}