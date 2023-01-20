using System;
using System.Linq;
using System.Reflection;
using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.Profiles.DataModels;

public class ProfileCategoryDataModel : DataModel, IDisposable
{
    private readonly ProfileCategory _profileCategory;

    public ProfileCategoryDataModel(ProfileCategory profileCategory)
    {
        _profileCategory = profileCategory;
        _profileCategory.ProfileConfigurationAdded += ProfileCategoryOnProfileConfigurationAdded;
        _profileCategory.ProfileConfigurationRemoved += ProfileCategoryOnProfileConfigurationRemoved;

        foreach (ProfileConfiguration profileConfiguration in _profileCategory.ProfileConfigurations)
            AddDynamicChild(profileConfiguration.ProfileId.ToString(), new ProfileConfigurationDataModel(profileConfiguration), profileConfiguration.Name);
    }

    public int ProfileCount => _profileCategory.ProfileConfigurations.Count;
    public int ActiveProfileCount => _profileCategory.ProfileConfigurations.Count(c => c.Profile != null);
    
    public override DataModelPropertyAttribute GetPropertyDescription(PropertyInfo propertyInfo)
    {
        return new DataModelPropertyAttribute {Name = _profileCategory.Name};
    }

    public void Dispose()
    {
        _profileCategory.ProfileConfigurationAdded -= ProfileCategoryOnProfileConfigurationAdded;
        _profileCategory.ProfileConfigurationRemoved -= ProfileCategoryOnProfileConfigurationRemoved;
    }

    private void ProfileCategoryOnProfileConfigurationAdded(object sender, ProfileConfigurationEventArgs e)
    {
        AddDynamicChild(e.ProfileConfiguration.ProfileId.ToString(), new ProfileConfigurationDataModel(e.ProfileConfiguration), e.ProfileConfiguration.Name);
    }

    private void ProfileCategoryOnProfileConfigurationRemoved(object sender, ProfileConfigurationEventArgs e)
    {
        RemoveDynamicChildByKey(e.ProfileConfiguration.ProfileId.ToString());
    }
}