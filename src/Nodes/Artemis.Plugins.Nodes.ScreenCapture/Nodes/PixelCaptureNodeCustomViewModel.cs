using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared.VisualScripting;
using ReactiveUI;

namespace Artemis.Plugins.Nodes.ScreenCapture.Nodes;

public class PixelCaptureNodeCustomViewModel : CustomNodeViewModel
{
    private readonly PixelCaptureNode _node;
    private readonly IInputService _inputService;
    private int _lastX;
    private int _lastY;
    private bool _captureNextClick;

    public PixelCaptureNodeCustomViewModel(PixelCaptureNode node, INodeScript script, IInputService inputService) : base(node, script)
    {
        _node = node;
        _inputService = inputService;

        NodeModified += (_, _) => this.RaisePropertyChanged(nameof(X));
        NodeModified += (_, _) => this.RaisePropertyChanged(nameof(Y));

        this.WhenActivated(d =>
        {
            // TODO: Leverage X and Y on the service once implemented instead of tracking manually
            _inputService.MouseMove += InputServiceOnMouseMove;
            _inputService.MouseButtonUp += InputServiceOnMouseButtonUp;

            Disposable.Create(() =>
            {
                _inputService.MouseMove -= InputServiceOnMouseMove;
                _inputService.MouseButtonUp -= InputServiceOnMouseButtonUp;
            }).DisposeWith(d);
        });
    }

    public int X
    {
        get => _node.Storage?.X ?? 0;
        set => _node.Storage = _node.Storage != null ? _node.Storage with {X = value} : new PixelCaptureNodeEntity(0, 0, null, value, 0);
    }

    public int Y
    {
        get => _node.Storage?.Y ?? 0;
        set => _node.Storage = _node.Storage != null ? _node.Storage with {Y = value} : new PixelCaptureNodeEntity(0, 0, null, 0, value);
    }

    public void CaptureNextClick()
    {
        _captureNextClick = true;
    }

    private void InputServiceOnMouseButtonUp(object? sender, ArtemisMouseButtonEventArgs e)
    {
        if (!_captureNextClick)
            return;

        _captureNextClick = false;
        _node.Storage = _node.Storage != null ? _node.Storage with {X = _lastX, Y = _lastY} : new PixelCaptureNodeEntity(0, 0, null, _lastX, _lastY);
    }

    private void InputServiceOnMouseMove(object? sender, ArtemisMouseMoveEventArgs e)
    {
        _lastX = e.CursorX;
        _lastY = e.CursorY;
    }
}