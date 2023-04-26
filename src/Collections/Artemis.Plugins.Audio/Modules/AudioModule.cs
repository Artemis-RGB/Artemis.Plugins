using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Audio.LayerEffects;
using Artemis.Plugins.Audio.LayerEffects.AudioProcessing.Spectrum;
using Artemis.Plugins.Audio.Modules.DataModels;
using Artemis.Plugins.Audio.Services;

namespace Artemis.Plugins.Audio.Modules;

[PluginFeature]
public class AudioModule : Module<AudioDataModel>
{
    private readonly AudioVisualizationService _audioVisualizationService;
    private int _useToken;

    public AudioModule(AudioVisualizationService audioVisualizationService)
    {
        _audioVisualizationService = audioVisualizationService;
    }

    public override void Enable()
    {
        _useToken = _audioVisualizationService.RegisterUse();
    }

    public override void Disable()
    {
        _audioVisualizationService.UnregisterUse(_useToken);
    }

    public override void Update(double deltaTime)
    {
        ISpectrumProvider provider = _audioVisualizationService.GetSpectrumProvider(Channel.Mix);
        ISpectrum spectrum = provider.GetLogarithmicSpectrum(24);
        
        foreach (Band band in spectrum)
        {
            AudioBandDataModel child = DataModel.GetOrAddDynamicChild(band);
            
            child.Min = band.Min;
            child.Max = band.Max;
            child.Average = band.Average;
            child.Sum = band.Sum;
        }
    }

    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();
}