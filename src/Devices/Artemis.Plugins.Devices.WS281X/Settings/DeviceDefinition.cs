namespace Artemis.Plugins.Devices.WS281X.Settings
{
    public class DeviceDefinition
    {
        public string Name { get; set; }
        public DeviceDefinitionType Type { get; set; }
        public string Port { get; set; }
    }

    public enum DeviceDefinitionType
    {
        Arduino,
        Bitwizard
    }
}