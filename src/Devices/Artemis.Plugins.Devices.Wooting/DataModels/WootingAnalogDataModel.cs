using Artemis.Core.Modules;
using RGB.NET.Core;
using System.Collections.Generic;

namespace Artemis.Plugins.Devices.Wooting.DataModels;

public class WootingAnalogDataModel : DataModel
{
    public double HighestAnalogValue { get; set; }

    private readonly Dictionary<LedId, DynamicChild<float>> _cache;

    public WootingAnalogDataModel()
    {
        _cache = new();
    }

    internal void SetAnalogValue(LedId key, float value)
    {
        if (!_cache.TryGetValue(key, out DynamicChild<float> keyDataModel))
        {
            keyDataModel = AddDynamicChild(key.ToString(), value);
            _cache.Add(key, keyDataModel);
        }

        keyDataModel.Value = value;
    }
}
