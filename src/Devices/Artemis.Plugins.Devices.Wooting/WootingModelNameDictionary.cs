using System.Collections.Generic;

namespace Artemis.Plugins.Devices.Wooting;

public static class WootingModelNameDictionary
{
    public static readonly Dictionary<string, string> WootingModelNames = new()
    {
        ["Wooting One"] = "WootingOne",
        ["Wooting Two HE"] = "WootingTwoHE",
        ["Wooting 60HE (ARM)"] = "Wooting 60HE (ARM)"
    };
}