using System.Reactive;
using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Ambilight.Screens;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.NodeEditor;
using Artemis.UI.Shared.VisualScripting;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Nodes.Screens;

public class CaptureScreenNodeCustomViewModel : CustomNodeViewModel
{
    #region Properties & Fields

    private readonly CaptureScreenNode _node;
    private readonly INodeEditorService _nodeEditorService;
    private readonly IWindowService _windowService;

    public ReactiveCommand<Unit, Unit> OpenConfigurationCommand { get; }

    #endregion

    #region Constructors

    /// <inheritdoc />
    public CaptureScreenNodeCustomViewModel(CaptureScreenNode node, INodeScript script, INodeEditorService nodeEditorService, IWindowService windowService)
        : base(node, script)
    {
        this._node = node;
        this._nodeEditorService = nodeEditorService;
        this._windowService = windowService;

        OpenConfigurationCommand = ReactiveCommand.Create(OpenConfiguration);
    }

    #endregion

    #region Methods

    private async void OpenConfiguration()
    {
        CaptureScreenNodeConfigurationDialogViewModel popupViewModel = new(_windowService);
        await _windowService.ShowDialogAsync(popupViewModel);
    }

    #endregion
}