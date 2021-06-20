using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.ScriptingProviders;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Declarations;
using Artemis.UI.Shared;
using Artemis.UI.Shared.ScriptingProviders;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels
{
    public class JavaScriptScriptEditorViewModel : ScriptEditorViewModel
    {
        private readonly IDataModelService _dataModelService;

        public JavaScriptScriptEditorViewModel(Plugin plugin, Script script, IDataModelService dataModelService) : base(script)
        {
            _dataModelService = dataModelService;
            DataModelDeclarations = GenerateDataModelJavaScript();
            EditorUri = new Uri(plugin.Directory.FullName + "\\Editor\\index.html");
        }

        public string DataModelDeclarations { get; }
        public Uri EditorUri { get; }

        public WebView2 EditorWebView { get; set; }

        protected override void OnViewLoaded()
        {
            EditorWebView = VisualTreeUtilities.FindChild<WebView2>(View, "EditorWebView");
            EditorWebView.NavigationCompleted += EditorWebViewOnNavigationCompleted;

            base.OnViewLoaded();
        }

        private async void EditorWebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await EditorWebView.ExecuteScriptAsync(
                $"monaco.languages.typescript.javascriptDefaults.addExtraLib(\"{HttpUtility.JavaScriptStringEncode(DataModelDeclarations)}\", 'dataModelDeclarations.d.ts');");
        }

        private string GenerateDataModelJavaScript()
        {
            List<TypeScriptDataModel> nameSpaces = new();

            List<DataModel> list = _dataModelService.GetDataModels();
            for (int index = 0; index < list.Count; index++)
            {
                nameSpaces.Add(new TypeScriptDataModel(list[index], index));
            }

            string namespaces = string.Join("\r\n", nameSpaces.Select(n => n.GenerateCode()));
            string rootClasses = string.Join("\r\n", nameSpaces.Select(n => $"  readonly {n.RootClass.Name}: {n.Name}.{n.RootClass.Name}"));
            string code = $@"
{namespaces}
declare class DataModelContainer {{
{rootClasses}
}}
const DataModel = new DataModelContainer();";
            return code;
        }
    }
}