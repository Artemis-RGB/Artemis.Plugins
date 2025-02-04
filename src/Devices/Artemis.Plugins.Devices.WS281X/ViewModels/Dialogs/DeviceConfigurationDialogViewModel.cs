using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Reactive;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.UI.Shared;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;

public class DeviceConfigurationDialogViewModel : ContentDialogViewModelBase
{
    private readonly DeviceDefinition _device;
    private readonly ObservableAsPropertyHelper<bool> _isUdpBased;
    private string _hostname;
    private string _name;
    private string _port;
    private int _speed;
    private DeviceDefinitionType? _type;

    public DeviceConfigurationDialogViewModel(DeviceDefinition device)
    {
        _device = device;
        
        _name = _device.Name;
        _type = _device.Type;
        _port = _device.Port;
        _hostname = _device.Hostname;
        _speed = _device.Speed;
        _isUdpBased = this.WhenAnyValue(vm => vm.Type, type => type == DeviceDefinitionType.ESP8266).ToProperty(this, vm => vm.IsUdpBased);

        SpeedList = [9600, 115200, 230400, 460800, 921600, 1500000];
        Ports = new ObservableCollection<string>(SerialPort.GetPortNames());
        this.ValidationRule(vm => vm.Type, type => type != null, "Device type is required");
        this.ValidationRule(vm => vm.Port, this.WhenAnyValue(vm => vm.IsUdpBased, vm => vm.Port, (udpBased, port) => udpBased || !string.IsNullOrWhiteSpace(port)), "Device port is required");
        this.ValidationRule(vm => vm.Hostname, this.WhenAnyValue(vm => vm.IsUdpBased, vm => vm.Hostname, (udpBased, hostname) => !udpBased || !string.IsNullOrWhiteSpace(hostname)), "A hostname is required");
        Accept = ReactiveCommand.Create(ExecuteAccept, ValidationContext.Valid);
    }

    public string Name
    {
        get => _name;
        set => RaiseAndSetIfChanged(ref _name, value);
    }

    public string Port
    {
        get => _port;
        set => RaiseAndSetIfChanged(ref _port, value);
    }

    public string Hostname
    {
        get => _hostname;
        set => RaiseAndSetIfChanged(ref _hostname, value);
    }

    public int Speed
    {
        get => _speed;
        set => RaiseAndSetIfChanged(ref _speed, value);
    }

    public DeviceDefinitionType? Type
    {
        get => _type;
        set => RaiseAndSetIfChanged(ref _type, value);
    }

    public bool IsUdpBased => _isUdpBased.Value;

    public ObservableCollection<string> Ports { get; }
    public ObservableCollection<int> SpeedList { get; }
    public ReactiveCommand<Unit, Unit> Accept { get; }
    
    private void ExecuteAccept()
    {
        if (HasErrors)
            return;

        if (!string.IsNullOrWhiteSpace(Name))
            _device.Name = Name;
        _device.Type = Type ?? DeviceDefinitionType.Arduino;
        _device.Port = Port;
        _device.Hostname = Hostname;
        _device.Speed = Speed;

        ContentDialog?.Hide(ContentDialogResult.Primary);
    }
}