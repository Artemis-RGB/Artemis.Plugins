using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Ambilight.ScreenCapture;
using ScreenCapture;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightBootstrapper : PluginBootstrapper
    {
        #region Properties & Fields

        internal static IScreenCaptureService ScreenCaptureService { get; private set; }

        #endregion

        #region Methods

        internal static void RefreshScreenCaptureService()
        {
            ScreenCaptureService.Dispose();
            ScreenCaptureService = null;
            ScreenCaptureService = new AmbilightScreenCaptureService(new DX11ScreenCaptureService());
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
            ScreenCaptureService ??= new AmbilightScreenCaptureService(new DX11ScreenCaptureService());
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
            ScreenCaptureService?.Dispose();
            ScreenCaptureService = null;
        }

        #endregion
    }
}
