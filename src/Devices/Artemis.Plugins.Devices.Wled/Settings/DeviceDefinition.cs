using Artemis.Core;

namespace Artemis.Plugins.Devices.Wled.Settings;

public class DeviceDefinition : CorePropertyChanged
{
    #region Properties & Fields

    private string _hostname;
    public string Hostname
    {
        get => _hostname;
        set => SetAndNotify(ref _hostname, value);
    }

    private string _manufacturer;
    public string Manufacturer
    {
        get => _manufacturer;
        set => SetAndNotify(ref _manufacturer, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => SetAndNotify(ref _model, value);
    }

    #endregion
}