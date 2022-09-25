using Artemis.Core.Modules;
using Artemis.Plugins.Devices.Wooting.Services;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Plugins.Devices.Wooting;

public class WootingModule : Module<WootingDataModel>
{
    private readonly WootingAnalogService _analogService;
    private readonly WootingProfileService _profileService;

    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

    public WootingModule(WootingAnalogService service, WootingProfileService profileService)
    {
        _analogService = service;
        _profileService = profileService;
    }

    public override void Enable()
    {
        foreach (var item in _analogService.Devices)
        {
            DataModel.AddDynamicChild(item.Info.device_name, new WootingDeviceDataModel());
        }

        AddTimedUpdate(TimeSpan.FromMilliseconds(250), UpdateKeyboardProfiles);
    }

    public override void Update(double deltaTime)
    {
        UpdateAnalogValues();
    }

    public override void Disable()
    {
    }

    private void UpdateAnalogValues()
    {
        _analogService.Update();
        foreach (var device in _analogService.Devices)
        {
            if (!DataModel.TryGetDynamicChild<WootingDeviceDataModel>(device.Info.device_name, out var deviceDataModel))
               continue;

            foreach (var item in device.AnalogValues)
            {
                deviceDataModel.Value.Analog.SetAnalogValue(item.Key, item.Value);
            }
        }
    }

    private void UpdateKeyboardProfiles(double deltaTime)
    {
        _profileService.Update();
        foreach (var device in _profileService.Devices)
        {
            if(!DataModel.TryGetDynamicChild<WootingDeviceDataModel>(device.Info.Model, out var deviceDataModel))
                continue;

            deviceDataModel.Value.Profile = device.Profile;
        }
    }
}
