using Artemis.UI.Shared;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureScreenViewModel : ActivatableViewModelBase
{
    private bool _isSelected;
    public Display Display { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public CaptureScreenViewModel(Display display)
    {
        Display = display;
        DisplayName = $"Display {Display.Index + 1}";
    }

    public void Update()
    {
    }
}