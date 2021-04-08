using System;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioCapture;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum
{
    public class FourierSpectrumProvider : AbstractAudioProcessor, ISpectrumProvider
    {
        #region Properties & Fields

        private readonly AudioBuffer _audioBuffer;

        private float[] _sampleDataMix;
        private float[] _sampleDataLeft;
        private float[] _sampleDataRight;
        private double[] _hamming;
        private Complex32[] _complexBuffer;

        private float[][] _spectrum;
        private int _usableDataLength;

        #endregion

        #region Constructors

        public FourierSpectrumProvider(AudioBuffer audioBuffer)
        {
            this._audioBuffer = audioBuffer;
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            _hamming = Window.Hamming(_audioBuffer.Size);
            _sampleDataMix = new float[_audioBuffer.Size];
            _sampleDataLeft = new float[_audioBuffer.Size];
            _sampleDataRight = new float[_audioBuffer.Size];
            _complexBuffer = new Complex32[_audioBuffer.Size];
            _usableDataLength = (_audioBuffer.Size / 2) + 1;
            _spectrum = new float[][] { new float[_usableDataLength], new float[_usableDataLength], new float[_usableDataLength] };
        }

        public override void Update()
        {
            _audioBuffer.CopyMixInto(ref _sampleDataMix, 0);
            _audioBuffer.CopyLeftInto(ref _sampleDataLeft, 0);
            _audioBuffer.CopyRightInto(ref _sampleDataRight, 0);

            ApplyHamming(ref _sampleDataMix, ref _sampleDataLeft, ref _sampleDataRight);

            CreateSpectrum(ref _sampleDataMix, ref _sampleDataLeft, ref _sampleDataRight);
        }

        private void ApplyHamming(ref float[] dataMix, ref float[] dataLeft, ref float[] dataRight)
        {
            for (int i = 0; i < dataMix.Length; i++)
                dataMix[i] = (float)(dataMix[i] * _hamming[i]);
            for (int i = 0; i < dataLeft.Length; i++)
                dataLeft[i] = (float)(dataLeft[i] * _hamming[i]);
            for (int i = 0; i < dataRight.Length; i++)
                dataRight[i] = (float)(dataRight[i] * _hamming[i]);
        }

        private void CreateSpectrum(ref float[] dataMix, ref float[] dataLeft, ref float[] dataRight)
        {
            // Center (mix) channel
            for (int i = 0; i < dataMix.Length; i++)
                _complexBuffer[i] = new Complex32(dataMix[i], 0);

            Fourier.Forward(_complexBuffer, FourierOptions.NoScaling);

            for (int i = 0; i < _spectrum[0].Length; i++)
            {
                Complex32 fourierData = _complexBuffer[i];
                _spectrum[0][i] = (float)Math.Sqrt(fourierData.Real * fourierData.Real) + (fourierData.Imaginary * fourierData.Imaginary);
            }

            // Left channel
            for (int i = 0; i < dataLeft.Length; i++)
                _complexBuffer[i] = new Complex32(dataLeft[i], 0);

            Fourier.Forward(_complexBuffer, FourierOptions.NoScaling);

            for (int i = 0; i < _spectrum[1].Length; i++)
            {
                Complex32 fourierData = _complexBuffer[i];
                _spectrum[1][i] = (float)Math.Sqrt(fourierData.Real * fourierData.Real) + (fourierData.Imaginary * fourierData.Imaginary);
            }

            // Right channel
            for (int i = 0; i < dataRight.Length; i++)
                _complexBuffer[i] = new Complex32(dataRight[i], 0);

            Fourier.Forward(_complexBuffer, FourierOptions.NoScaling);

            for (int i = 0; i < _spectrum[2].Length; i++)
            {
                Complex32 fourierData = _complexBuffer[i];
                _spectrum[2][i] = (float)Math.Sqrt(fourierData.Real * fourierData.Real) + (fourierData.Imaginary * fourierData.Imaginary);
            }
        }

        public ISpectrum GetLinearSpectrum(int bands = 64, float minFrequency = -1, float maxFrequency = -1, int audioChannel = 1) => new LinearSpectrum(_spectrum, bands, minFrequency, maxFrequency, audioChannel);

        public ISpectrum GetLogarithmicSpectrum(int bands = 12, float minFrequency = -1, float maxFrequency = -1, int audioChannel = 1) => new LogarithmicSpectrum(_spectrum, bands, minFrequency, maxFrequency, audioChannel);

        public ISpectrum GetGammaSpectrum(int bands = 64, float gamma = 2, float minFrequency = -1, float maxFrequency = -1, int audioChannel = 1) => new GammaSpectrum(_spectrum, bands, gamma, minFrequency, maxFrequency, audioChannel);

        public ISpectrum GetRawSpectrum(int audioChannel = 1) => new RawSpectrumProvider(_spectrum, audioChannel);

        #endregion
    }
}
