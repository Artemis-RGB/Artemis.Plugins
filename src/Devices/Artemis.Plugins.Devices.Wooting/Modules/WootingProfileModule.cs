using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Devices.Wooting.DataModels;
using Artemis.Plugins.Devices.Wooting.Services.ProfileService;

namespace Artemis.Plugins.Devices.Wooting.Modules;

public class WootingProfileModule : Module<WootingDataModel>
{
    private readonly WootingProfileService _profileService;
    public override List<IModuleActivationRequirement> ActivationRequirements { get; } = new();
    private int _useToken;

    public WootingProfileModule(WootingProfileService service)
    {
        _profileService = service;
    }

    public override void Enable()
    {
        _useToken = _profileService.RegisterUse();
        for (int index = 0; index < _profileService.Devices.Count; index++)
        {
            WootingProfileDevice item = _profileService.Devices[index];
            DataModel.AddDynamicChild(index.ToString(), new WootingProfileDataModel(), item.Info.Model);
        }
    }

    public override void Update(double deltaTime)
    {
        UpdateProfiles();
    }

    public override void Disable()
    {
        _profileService.UnregisterUse(_useToken);
    }

    private void UpdateProfiles()
    {
        _profileService.Update();
        for (int index = 0; index < _profileService.Devices.Count; index++)
        {
            WootingProfileDevice device = _profileService.Devices[index];
            if (!DataModel.TryGetDynamicChild(index.ToString(), out DynamicChild<WootingProfileDataModel> deviceDataModel))
                continue;

            deviceDataModel.Value.Profile = device.Profile;
        }
    }
}