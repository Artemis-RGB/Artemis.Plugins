using Newtonsoft.Json;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace Artemis.Plugins.PhilipsHue.Models;

public class PhilipsHueBridge
{
    public PhilipsHueBridge()
    {
    }

    public PhilipsHueBridge(LocatedBridge newBridge)
    {
        BridgeId = newBridge.BridgeId;
        IpAddress = newBridge.IpAddress;
    }

    [JsonIgnore]
    public ILocalHueClient Client { get; set; }

    [JsonIgnore]
    public Bridge BridgeInfo { get; set; }

    public string BridgeId { get; set; }
    public string IpAddress { get; set; }
    public string AppKey { get; set; }
    public string StreamingClientKey { get; set; }
}