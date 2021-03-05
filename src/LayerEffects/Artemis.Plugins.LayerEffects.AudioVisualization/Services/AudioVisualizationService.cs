using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioCapture;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.Services
{
    public class AudioVisualizationService: IPluginService, IDisposable
    {
        private readonly ICoreService _coreService;

        public AudioVisualizationService(ICoreService coreService)
        {
            _coreService = coreService;
        }

        #region Properties & Fields

        private bool _isActivated;
        private int _useToken;
        private readonly HashSet<int> _useTokens = new();

        private IAudioInput _audioInput;
        private AudioBuffer _audioBuffer;

        internal ISpectrumProvider SpectrumProvider { get; private set; }

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

            _audioInput = new CSCoreAudioInput();
            _audioInput.Initialize();

            _audioBuffer = new AudioBuffer(4096); // Working with ~93ms
            _audioInput.DataAvailable += (left, right) => _audioBuffer.Put(left, right);

            SpectrumProvider = new FourierSpectrumProvider(_audioBuffer);
            SpectrumProvider.Initialize();

            _coreService.FrameRendering += Update;

            _isActivated = true;
        }

        private void Deactivate()
        {
            if (!_isActivated) return;

            _coreService.FrameRendering -= Update;

            SpectrumProvider.Dispose();
            _audioInput.Dispose();

            SpectrumProvider = null;
            _audioBuffer = null;
            _audioInput = null;

            _isActivated = false;
        }

        private void Update(object sender, FrameRenderingEventArgs args)
        {
            SpectrumProvider?.Update();
        }

        public void Dispose()
        {
            Deactivate();
            _useTokens.Clear();
        }

        #endregion
    }
}