using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Devices.Wooting.DataModels;
using Artemis.Plugins.Devices.Wooting.Services.ProfileService;

namespace Artemis.Plugins.Devices.Wooting.Modules;

public class WootingProfileModule : Module<WootingDataModel>
{
    private readonly WootingProfileService _profileService;
    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();

    public WootingProfileModule(WootingProfileService service)
    {
        _profileService = service;
    }

    public override void Enable()
    {
        foreach (WootingProfileDevice item in _profileService.Devices)
            DataModel.AddDynamicChild(item.Info.Model, new WootingProfileDataModel());
    }

    public override void Update(double deltaTime)
    {
        UpdateProfiles();
    }

    public override void Disable()
    {
    }

    private void UpdateProfiles()
    {
        _profileService.Update();
        foreach (WootingProfileDevice device in _profileService.Devices)
        {
            if (!DataModel.TryGetDynamicChild(device.Info.Model, out DynamicChild<WootingProfileDataModel> deviceDataModel))
                continue;
            
            deviceDataModel.Value.Profile = device.Profile;
        }
    }
}