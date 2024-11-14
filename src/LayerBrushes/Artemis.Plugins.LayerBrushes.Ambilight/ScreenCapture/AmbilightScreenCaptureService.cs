using System.Collections.Generic;
using System.Linq;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.ScreenCapture
{
    public sealed class AmbilightScreenCaptureService : IScreenCaptureService
    {
        #region Properties & Fields

        private readonly IScreenCaptureService _screenCaptureService;

        private readonly Dictionary<Display, IScreenCapture> _screenCaptures = [];

        private readonly List<GraphicsCard> _graphicsCards;
        private readonly Dictionary<GraphicsCard, List<Display>> _displays = [];

        #endregion

        #region Constructors

        public AmbilightScreenCaptureService(IScreenCaptureService screenCaptureService)
        {
            this._screenCaptureService = screenCaptureService;

            _graphicsCards = _screenCaptureService.GetGraphicsCards().ToList();

            foreach (GraphicsCard graphicsCard in _graphicsCards)
                _displays.Add(graphicsCard, _screenCaptureService.GetDisplays(graphicsCard).ToList());
        }

        #endregion

        #region Methods

        public IEnumerable<GraphicsCard> GetGraphicsCards() => _graphicsCards;

        public IEnumerable<Display> GetDisplays(GraphicsCard graphicsCard) => _displays.TryGetValue(graphicsCard, out List<Display>? displays) ? displays : Enumerable.Empty<Display>();

        public IScreenCapture GetScreenCapture(Display display)
        {
            lock (_screenCaptures)
            {
                if (!_screenCaptures.TryGetValue(display, out IScreenCapture? screenCapture))
                    _screenCaptures.Add(display, screenCapture = new AmbilightScreenCapture(_screenCaptureService.GetScreenCapture(display)));

                return screenCapture;
            }
        }

        public void Dispose()
        {
            foreach (IScreenCapture screenCapture in _screenCaptures.Values)
                screenCapture.Dispose();
            _screenCaptures.Clear();

            _screenCaptureService.Dispose();
        }

        #endregion
    }
}
