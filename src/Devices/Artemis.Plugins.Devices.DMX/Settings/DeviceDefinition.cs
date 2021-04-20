using System.Collections.Generic;
using Artemis.Core;

namespace Artemis.Plugins.Devices.DMX.Settings
{
    public class DeviceDefinition : CorePropertyChanged
    {
        private string _hostname;
        private string _manufacturer;
        private string _model;
        private string _name;
        private int _port;
        private short _universe;

        public DeviceDefinition()
        {
            LedDefinitions = new List<LedDefinition>();
        }

        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        public string Hostname
        {
            get => _hostname;
            set => SetAndNotify(ref _hostname, value);
        }

        public int Port
        {
            get => _port;
            set => SetAndNotify(ref _port, value);
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set => SetAndNotify(ref _manufacturer, value);
        }

        public string Model
        {
            get => _model;
            set => SetAndNotify(ref _model, value);
        }

        public short Universe
        {
            get => _universe;
            set => SetAndNotify(ref _universe, value);
        }

        public List<LedDefinition> LedDefinitions { get; set; }
    }
}