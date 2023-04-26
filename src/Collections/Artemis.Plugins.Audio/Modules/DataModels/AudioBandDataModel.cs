using Artemis.Core.Modules;

namespace Artemis.Plugins.Audio.Modules.DataModels;

public class AudioBandDataModel : DataModel
{
    public float Min { get; set; }
    public float Max { get; set; }
    public float Average { get; set; }
    public float Sum { get; set; }
}