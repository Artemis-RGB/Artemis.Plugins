using Artemis.UI.Shared;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureScreenViewModel : ActivatableViewModelBase
{
    public Display Display { get; }

    public CaptureScreenViewModel(Display display)
    {
        Display = display;
        DisplayName = $"Display {Display.Index + 1}";
    }

    public void Update()
    {
        
    }
}