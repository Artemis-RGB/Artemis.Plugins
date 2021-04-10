using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Ambilight.ScreenCapture;
using ScreenCapture;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightBootstrapper : IPluginBootstrapper
    {
        #region Properties & Fields

        internal static IScreenCaptureService ScreenCaptureService { get; private set; }

        #endregion

        #region Methods

        public void Enable(Plugin plugin)
        {
            ScreenCaptureService ??= new AmbilightScreenCaptureService(new DX11ScreenCaptureService());
        }

        public void Disable(Plugin plugin)
        {
            ScreenCaptureService?.Dispose();
            ScreenCaptureService = null;
        }

        #endregion
    }
}
