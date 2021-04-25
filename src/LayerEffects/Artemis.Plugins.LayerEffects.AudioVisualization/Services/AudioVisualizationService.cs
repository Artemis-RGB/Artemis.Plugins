using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioCapture;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.Services
{
    public class AudioVisualizationService : IPluginService, IDisposable
    {
        #region Properties & Fields

        private readonly ICoreService _coreService;

        private bool _isActivated;
        private int _useToken;
        private readonly HashSet<int> _useTokens = new();

        private IAudioInput _audioInput;
        private AudioBuffer _audioBuffer;

        private readonly Dictionary<Channel, ISpectrumProvider> _spectrumProviders = new();

        #endregion

        #region Constructors

        public AudioVisualizationService(ICoreService coreService)
        {
            this._coreService = coreService;
        }

        #endregion

        #region Methods

        // TODO: The shared data API should offer something similar
        internal int RegisterUse()
        {
            Activate();
            int useToken = ++_useToken;
            _useTokens.Add(useToken);
            return useToken;
        }

        // TODO: The shared data API should offer something similar
        internal void UnregisterUse(int useToken)
        {
            _useTokens.Remove(useToken);
            if (_useTokens.Count == 0)
                Deactivate();
        }

        private void Activate()
        {
            if (_isActivated) return;

            _audioInput = new NAudioAudioInput();
            _audioInput.Initialize();

            _audioBuffer = new AudioBuffer(4096); // Working with ~93ms
            _audioInput.DataAvailable += (left, right) => _audioBuffer.Put(left, right);

            _spectrumProviders.Add(Channel.Mix, new FourierSpectrumProvider(new SpectrumAudioDataProvider(_audioBuffer, Channel.Mix)));
            _spectrumProviders.Add(Channel.Left, new FourierSpectrumProvider(new SpectrumAudioDataProvider(_audioBuffer, Channel.Left)));
            _spectrumProviders.Add(Channel.Right, new FourierSpectrumProvider(new SpectrumAudioDataProvider(_audioBuffer, Channel.Right)));

            foreach (ISpectrumProvider spectrumProvider in _spectrumProviders.Values)
                spectrumProvider.Initialize();

            _coreService.FrameRendering += Update;

            _isActivated = true;
        }

        private void Deactivate()
        {
            if (!_isActivated) return;

            _coreService.FrameRendering -= Update;

            foreach (ISpectrumProvider spectrumProvider in _spectrumProviders.Values)
                spectrumProvider.Dispose();
            _audioInput.Dispose();

            _spectrumProviders.Clear();
            _audioBuffer = null;
            _audioInput = null;

            _isActivated = false;
        }

        private void Update(object sender, FrameRenderingEventArgs args)
        {
            foreach (ISpectrumProvider spectrumProvider in _spectrumProviders.Values)
                spectrumProvider?.Update(); //DarthAffe 10.04.2021: This is not updating, it's used more like a mark as dirty
        }

        public ISpectrumProvider GetSpectrumProvider(Channel channel) => _spectrumProviders.TryGetValue(channel, out ISpectrumProvider spectrumProvider) ? spectrumProvider : null;

        public void Dispose()
        {
            Deactivate();
            _useTokens.Clear();
        }

        #endregion
    }
}