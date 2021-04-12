using System;
using Artemis.Core.LayerEffects;
using Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum;
using Artemis.Plugins.LayerEffects.AudioVisualization.Extensions;
using Artemis.Plugins.LayerEffects.AudioVisualization.Services;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.AudioVisualization
{
    public class AudioVisualizationEffect : LayerEffect<AudioVisualizationEffectProperties>
    {
        private readonly AudioVisualizationService _audioVisualizationService;

        public AudioVisualizationEffect(AudioVisualizationService audioVisualizationService)
        {
            _audioVisualizationService = audioVisualizationService;
        }

        #region Properties & Fields

        private int _managerUseToken;

        private float _lastSmoothing;
        private double _smoothingFactor;
        private float _lastEmphasise;
        private double _emphasiseFactor;

        private float[] _visualizationData;

        #endregion

        #region Methods

        public override void EnableLayerEffect()
        {
            _managerUseToken = _audioVisualizationService.RegisterUse();
        }

        public override void DisableLayerEffect()
        {
            _audioVisualizationService.UnregisterUse(_managerUseToken);
        }

        public SKPath CreateShapeClip(SKRect renderBounds)
        {
            SKPath visualizationPath = new();
            float barWidth = renderBounds.Width / _visualizationData.Length;
            for (int i = 0; i < _visualizationData.Length; i++)
            {
                float value = _visualizationData[i];
                visualizationPath.AddRect(new SKRect(
                    (i * barWidth) + renderBounds.Left,
                    (renderBounds.Height - (renderBounds.Height * value)) + renderBounds.Top,
                    ((i + 1) * barWidth) + renderBounds.Left,
                    renderBounds.Height + renderBounds.Top)
                );
            }

            return visualizationPath;
        }


        public override void PreProcess(SKCanvas canvas, SKRect renderBounds, SKPaint paint)
        {
            canvas.ClipPath(CreateShapeClip(renderBounds));
        }

        public override void PostProcess(SKCanvas canvas, SKRect renderBounds, SKPaint paint)
        {
        }

        public override void Update(double deltaTime)
        {
            ISpectrumProvider spectrumProvider = _audioVisualizationService.GetSpectrumProvider(Properties.Channel);
            if (spectrumProvider == null) return;

            RecalculateConfigValues();

            ISpectrum spectrum = GetSpectrum(spectrumProvider);
            if (spectrum == null) return;

            for (int i = 0; i < spectrum.BandCount; i++)
            {
                double binPower = GetBandValue(spectrum[i]);

                binPower = Math.Max(0, 20 * Math.Log10(binPower));

                binPower = Math.Max(0, binPower);
                binPower /= Properties.ReferenceLevel.CurrentValue;
                if (Properties.EmphasisePeaks.CurrentValue > 0.001)
                    binPower = Math.Pow(binPower, 1 + Properties.EmphasisePeaks.CurrentValue) * _emphasiseFactor;

                if (i < _visualizationData.Length)
                {
                    _visualizationData[i] = (float)((_visualizationData[i] * _smoothingFactor) + (binPower * (1.0 - _smoothingFactor))).Clamp(0, 1);
                    if (float.IsNaN(_visualizationData[i])) _visualizationData[i] = 0;
                }
            }
        }

        private ISpectrum GetSpectrum(ISpectrumProvider spectrumProvider)
        {
            return Properties.SpectrumMode.CurrentValue switch
            {
                SpectrumMode.Gamma => spectrumProvider.GetGammaSpectrum(Properties.Bars.CurrentValue, Properties.Gamma.CurrentValue, Properties.MinFrequency.CurrentValue, Properties.MaxFrequency.CurrentValue),
                SpectrumMode.Logarithmic => spectrumProvider.GetLogarithmicSpectrum(Properties.Bars.CurrentValue, Properties.MinFrequency.CurrentValue, Properties.MaxFrequency.CurrentValue),
                SpectrumMode.Linear => spectrumProvider.GetLinearSpectrum(Properties.Bars.CurrentValue, Properties.MinFrequency.CurrentValue, Properties.MaxFrequency.CurrentValue),
                _ => null
            };
        }

        private float GetBandValue(Band band)
        {
            return Properties.ValueMode.CurrentValue switch
            {
                ValueMode.Max => band.Max,
                ValueMode.Average => band.Average,
                ValueMode.Sum => band.Sum,
                _ => 0
            };
        }

        private void RecalculateConfigValues()
        {
            if ((_visualizationData == null) || (_visualizationData.Length != Properties.Bars.CurrentValue))
                _visualizationData = new float[Properties.Bars.CurrentValue];

            if (Math.Abs(_lastSmoothing - Properties.Smoothing.CurrentValue) > 0.0001f)
            {
                _smoothingFactor = Math.Log10(Properties.Smoothing.CurrentValue);
                _lastSmoothing = Properties.Smoothing.CurrentValue;
            }

            if (Math.Abs(_lastEmphasise - Properties.EmphasisePeaks.CurrentValue) > 0.0001f)
            {
                _emphasiseFactor = 0.75 * (1 + Properties.EmphasisePeaks.CurrentValue);
                _lastEmphasise = Properties.EmphasisePeaks.CurrentValue;
            }
        }

        #endregion
    }
}