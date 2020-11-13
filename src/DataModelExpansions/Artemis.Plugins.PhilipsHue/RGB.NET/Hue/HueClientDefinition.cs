namespace Artemis.Plugins.PhilipsHue.RGB.NET.Hue
{
    public class HueClientDefinition
    {
        public HueClientDefinition(string ip, string appKey, string clientKey)
        {
            Ip = ip;
            AppKey = appKey;
            ClientKey = clientKey;
        }

        public string Ip { get; }
        public string AppKey { get; }
        public string ClientKey { get; }
    }
}