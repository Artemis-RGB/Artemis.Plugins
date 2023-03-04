using System;
using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Devices.Wooting.DataModels;
using Artemis.Plugins.Devices.Wooting.Services.AnalogService;
using Artemis.Plugins.Devices.Wooting.Services.ProfileService;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Wooting.Modules;

public class WootingModule : Module<WootingDataModel>
{
    private readonly WootingAnalogService _analogService;
    private readonly WootingProfileService _profileService;
    private double _timeSinceLastUpdate;

    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

    public WootingModule(WootingAnalogService service, WootingProfileService profileService)
    {
        _analogService = service;
        _profileService = profileService;
    }

    public override void Enable()
    {
        foreach (WootingAnalogDevice item in _analogService.Devices)
            DataModel.AddDynamicChild(item.Info.device_name, new WootingDeviceDataModel());
    }

    public override void Update(double deltaTime)
    {
        _timeSinceLastUpdate += deltaTime;
        if (_timeSinceLastUpdate > 0.3d)
        {
            _timeSinceLastUpdate = 0;
            UpdateKeyboardProfiles(deltaTime);
        }
        
        UpdateAnalogValues();
    }

    public override void Disable()
    {
    }

    private void UpdateAnalogValues()
    {
        _analogService.Update();
        foreach (WootingAnalogDevice device in _analogService.Devices)
        {
            if (!DataModel.TryGetDynamicChild<WootingDeviceDataModel>(device.Info.device_name, out DynamicChild<WootingDeviceDataModel> deviceDataModel))
               continue;

            double highest = double.MinValue;
            foreach (KeyValuePair<LedId, float> item in device.AnalogValues)
            {
                highest = Math.Max(highest, item.Value);
                deviceDataModel.Value.Analog.SetAnalogValue(item.Key, item.Value);
            }
            deviceDataModel.Value.Analog.HighestAnalogValue = highest;
        }
    }

    private void UpdateKeyboardProfiles(double deltaTime)
    {
        _profileService.Update();
        foreach (WootingProfileDevice device in _profileService.Devices)
        {
            if (!WootingModelNameDictionary.WootingModelNames.TryGetValue(device.Info.Model, out string modelName))
                continue;
            
            if (!DataModel.TryGetDynamicChild(modelName, out DynamicChild<WootingDeviceDataModel> deviceDataModel))
                continue;

            deviceDataModel.Value.Profile = device.Profile;
        }
    }
}