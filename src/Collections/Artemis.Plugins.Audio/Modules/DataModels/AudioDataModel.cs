using System;
using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Audio.LayerEffects.AudioProcessing.Spectrum;
using MathNet.Numerics;

namespace Artemis.Plugins.Audio.Modules.DataModels;

public class AudioDataModel : DataModel
{
    private readonly Dictionary<float, AudioBandDataModel> _bands = new();
    
    public AudioBandDataModel GetOrAddDynamicChild(Band band)
    {
        if (_bands.TryGetValue(band.CenterFrequency, out AudioBandDataModel bandDataModel)) return bandDataModel;
        
        DynamicChild<AudioBandDataModel> child = AddDynamicChild<AudioBandDataModel>(
            Math.Round(band.CenterFrequency).ToString(),
            new(),
            name: band.CenterFrequency.ToString());
        _bands.Add(band.CenterFrequency, child.Value);

        return child.Value;
    }
}