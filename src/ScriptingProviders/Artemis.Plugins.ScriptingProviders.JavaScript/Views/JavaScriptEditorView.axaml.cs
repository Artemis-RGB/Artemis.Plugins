using Artemis.Plugins.ScriptingProviders.JavaScript.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Views;

public partial class JavaScriptEditorView : ReactiveUserControl<JavaScriptEditorViewModel>
{
    public JavaScriptEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}