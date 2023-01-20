using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.Profiles.DataModels;

public class ProfileConfigurationDataModel : DataModel
{
    private readonly ProfileConfiguration _profileConfiguration;

    public ProfileConfigurationDataModel(ProfileConfiguration profileConfiguration)
    {
        _profileConfiguration = profileConfiguration;
    }

    public bool IsLoaded => _profileConfiguration.Profile != null;
    public bool IsSuspended => _profileConfiguration.IsSuspended;
    public bool ActivationConditionMet => _profileConfiguration.ActivationConditionMet;
}