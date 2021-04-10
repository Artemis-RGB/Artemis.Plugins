using System;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum
{
    public class FourierSpectrumProvider : AbstractAudioProcessor, ISpectrumProvider
    {
        #region Properties & Fields

        private readonly IAudioDataProvider _dataProvider;

        private float[] _sampleData;
        private double[] _hamming;
        private Complex32[] _complexBuffer;

        private float[] _spectrum;
        private int _usableDataLength;

        private bool _isDirty = true;

        #endregion

        #region Constructors

        public FourierSpectrumProvider(IAudioDataProvider dataProvider)
        {
            this._dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            _hamming = Window.Hamming(_dataProvider.BufferSize);
            _sampleData = new float[_dataProvider.BufferSize];
            _complexBuffer = new Complex32[_dataProvider.BufferSize];
            _usableDataLength = (_dataProvider.BufferSize / 2) + 1;
            _spectrum = new float[_usableDataLength];
        }

        public override void Update()
        {
            _isDirty = true;
        }

        private void CalculateSpectrum()
        {
            if (!_isDirty) return;

            Span<float> data = new(_sampleData);
            _dataProvider.GetDataInto(data);

            ApplyHamming(data);
            CreateSpectrum(data);

            _isDirty = false;
        }

        private void ApplyHamming(in Span<float> data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = (float)(data[i] * _hamming[i]);
        }

        private void CreateSpectrum(in Span<float> data)
        {
            for (int i = 0; i < data.Length; i++)
                _complexBuffer[i] = new Complex32(data[i], 0);

            Fourier.Forward(_complexBuffer, FourierOptions.NoScaling);

            for (int i = 0; i < _spectrum.Length; i++)
            {
                Complex32 fourierData = _complexBuffer[i];
                _spectrum[i] = (float)Math.Sqrt(fourierData.Real * fourierData.Real) + (fourierData.Imaginary * fourierData.Imaginary);
            }
        }

        public ISpectrum GetLinearSpectrum(int bands = 64, float minFrequency = -1, float maxFrequency = -1)
        {
            CalculateSpectrum();
            return new LinearSpectrum(_spectrum, bands, minFrequency, maxFrequency);
        }

        public ISpectrum GetLogarithmicSpectrum(int bands = 12, float minFrequency = -1, float maxFrequency = -1)
        {
            CalculateSpectrum();
            return new LogarithmicSpectrum(_spectrum, bands, minFrequency, maxFrequency);
        }

        public ISpectrum GetGammaSpectrum(int bands = 64, float gamma = 2, float minFrequency = -1, float maxFrequency = -1)
        {
            CalculateSpectrum();
            return new GammaSpectrum(_spectrum, bands, gamma, minFrequency, maxFrequency);
        }

        public ISpectrum GetRawSpectrum()
        {
            CalculateSpectrum();
            return new RawSpectrumProvider(_spectrum);
        }

        #endregion
    }
}
