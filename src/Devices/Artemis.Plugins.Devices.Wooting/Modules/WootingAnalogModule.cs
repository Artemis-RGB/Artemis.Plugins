using System;
using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Devices.Wooting.DataModels;
using Artemis.Plugins.Devices.Wooting.Services.AnalogService;
using Artemis.Plugins.Devices.Wooting.Services.ProfileService;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Wooting.Modules;

public class WootingAnalogModule : Module<WootingDataModel>
{
    private readonly WootingAnalogService _analogService;
    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();
    private int _useToken;

    public WootingAnalogModule(WootingAnalogService service)
    {
        _analogService = service;
    }

    public override void Enable()
    {
        _useToken = _analogService.RegisterUse();
        foreach (WootingAnalogDevice item in _analogService.Devices)
            DataModel.AddDynamicChild(item.Info.device_name, new WootingAnalogDataModel());
    }

    public override void Update(double deltaTime)
    {
        UpdateAnalogValues();
    }

    public override void Disable()
    {
        _analogService.UnregisterUse(_useToken);
        DataModel.ClearDynamicChildren();
    }

    private void UpdateAnalogValues()
    {
        _analogService.Update();
        foreach (WootingAnalogDevice device in _analogService.Devices)
        {
            if (!DataModel.TryGetDynamicChild(device.Info.device_name, out DynamicChild<WootingAnalogDataModel> deviceDataModel))
               continue;

            double highest = double.MinValue;
            foreach (KeyValuePair<LedId, float> item in device.AnalogValues)
            {
                highest = Math.Max(highest, item.Value);
                deviceDataModel.Value.SetAnalogValue(item.Key, item.Value);
            }
            deviceDataModel.Value.HighestAnalogValue = highest;
        }
    }

}