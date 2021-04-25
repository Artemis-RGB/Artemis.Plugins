using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Plugins.Devices.DMX.Settings;
using Artemis.UI.Shared.Services;
using FluentValidation;
using RGB.NET.Core;
using Stylet;

namespace Artemis.Plugins.Devices.DMX.ViewModels.Dialogs
{
    public class DeviceConfigurationDialogViewModel : Screen
    {
        private readonly DeviceDefinition _device;
        private readonly IDialogService _dialogService;
        private string _hostname;
        private string _manufacturer;
        private string _model;
        private string _name;
        private int _port;
        private int _universe;

        public DeviceConfigurationDialogViewModel(DeviceDefinition device, IModelValidator<DeviceConfigurationDialogViewModel> validator, IDialogService dialogService) :
            base(validator)
        {
            _device = device;
            _dialogService = dialogService;

            Device = device;
            Name = _device.Name;
            Hostname = _device.Hostname;
            Port = _device.Port;
            Manufacturer = _device.Manufacturer;
            Model = _device.Model;
            Universe = _device.Universe;

            LedDefinitions = new BindableCollection<LedDefinition>(device.LedDefinitions);
            LedIds = Enum.GetValues<LedId>().ToList();
        }

        public DeviceDefinition Device { get; }
        public List<LedId> LedIds { get; }

        public BindableCollection<LedDefinition> LedDefinitions { get; }

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

        public int Universe
        {
            get => _universe;
            set => SetAndNotify(ref _universe, value);
        }

        public void AddLed()
        {
            if (!LedDefinitions.Any())
            {
                LedDefinitions.Add(new LedDefinition
                {
                    LedId = LedId.LedStripe1,
                    R = 0,
                    G = 1,
                    B = 2
                });
            }
            else
            {
                LedDefinition previous = LedDefinitions.Last();
                LedId ledId = LedDefinitions.Max(l => l.LedId);
                ledId = ledId.Next();

                LedDefinitions.Add(new LedDefinition
                {
                    LedId = ledId,
                    R = previous.R + 3,
                    G = previous.G + 3,
                    B = previous.B + 3
                });
            }
        }

        public async Task AddLeds()
        {
            object result = await _dialogService.ShowDialogAt<AddLedsDialogViewModel>("AddLedsDialog");
            if (result is int intResult)
                for (int i = 0; i < intResult; i++)
                    AddLed();
        }

        public void DeleteLed(LedDefinition ledDefinition)
        {
            LedDefinitions.Remove(ledDefinition);
        }

        public async Task ClearLeds()
        {
            bool confirmed = await _dialogService.ShowConfirmDialogAt("AddLedsDialog", "Clear LEDs", "Are you sure you want to clear out all LEDs?");
            if (!confirmed)
                return;
            LedDefinitions.Clear();
        }

        public async Task Accept()
        {
            await ValidateAsync();

            if (HasErrors)
                return;

            _device.Name = Name;
            _device.Hostname = Hostname;
            _device.Port = Port;
            _device.Manufacturer = Manufacturer;
            _device.Model = Model;
            _device.Universe = (short) Universe;
            _device.LedDefinitions.Clear();
            _device.LedDefinitions.AddRange(LedDefinitions);

            RequestClose(true);
        }

        public void Cancel()
        {
            RequestClose(false);
        }
    }

    public class DeviceConfigurationDialogViewModelValidator : AbstractValidator<DeviceConfigurationDialogViewModel>
    {
        public DeviceConfigurationDialogViewModelValidator()
        {
            RuleFor(m => m.Hostname).NotEmpty().WithMessage("A hostname is required");
            RuleFor(m => m.Port).NotEmpty().WithMessage("Device port is required");
            RuleFor(m => m.Universe).GreaterThanOrEqualTo(1).WithMessage("Universe must range from 1 to 63999");
            RuleFor(m => m.Universe).LessThanOrEqualTo(63999).WithMessage("Universe must range from 1 to 63999");
        }
    }
}

public static class Extensions
{
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[]) Enum.GetValues(src.GetType());
        int j = Array.IndexOf(Arr, src) + 1;
        return Arr.Length == j ? Arr[0] : Arr[j];
    }
}