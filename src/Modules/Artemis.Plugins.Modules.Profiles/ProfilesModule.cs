using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Profiles.DataModels;

namespace Artemis.Plugins.Modules.Profiles;

public class ProfilesModule : Module<ProfilesDataModel>
{
    private readonly IProfileService _profileService;

    public ProfilesModule(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    public override void Enable()
    {
        _profileService.ProfileCategoryAdded += ProfileServiceOnProfileCategoryAdded;
        _profileService.ProfileCategoryRemoved += ProfileServiceOnProfileCategoryRemoved;

        foreach (ProfileCategory profileCategory in _profileService.ProfileCategories)
            DataModel.AddDynamicChild(profileCategory.EntityId.ToString(), new ProfileCategoryDataModel(profileCategory), profileCategory.Name);
    }

    public override void Disable()
    {
        _profileService.ProfileCategoryAdded -= ProfileServiceOnProfileCategoryAdded;
        _profileService.ProfileCategoryRemoved -= ProfileServiceOnProfileCategoryRemoved;

        List<ProfileCategoryDataModel> dataModels = DataModel.DynamicChildren
            .Where(c => c.Value.BaseValue is ProfileCategoryDataModel)
            .Select(c => c.Value.BaseValue)
            .Cast<ProfileCategoryDataModel>()
            .ToList();
        DataModel.ClearDynamicChildren();
        foreach (ProfileCategoryDataModel profileCategoryDataModel in dataModels)
            profileCategoryDataModel.Dispose();
    }

    public override DataModelPropertyAttribute GetDataModelDescription()
    {
        return new DataModelPropertyAttribute
        {
            Name = "Artemis Profiles",
            Description = "A data model containing all your Artemis profiles organized by category"
        };
    }

    public override void Update(double deltaTime)
    {
    }

    private void ProfileServiceOnProfileCategoryAdded(object sender, ProfileCategoryEventArgs e)
    {
        DataModel.AddDynamicChild(e.ProfileCategory.EntityId.ToString(), new ProfileCategoryDataModel(e.ProfileCategory), e.ProfileCategory.Name);
    }

    private void ProfileServiceOnProfileCategoryRemoved(object sender, ProfileCategoryEventArgs e)
    {
        DynamicChild<ProfileCategoryDataModel> dataModel = DataModel.GetDynamicChild<ProfileCategoryDataModel>(e.ProfileCategory.EntityId.ToString());
        DataModel.RemoveDynamicChild(dataModel);
        dataModel.Value.Dispose();
    }
}