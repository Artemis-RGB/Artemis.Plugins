using System;
using Artemis.Core;

namespace Artemis.Plugins.WebAPI.Models
{
    public class ProfileConfigurationModel
    {
        public ProfileConfigurationModel(ProfileConfiguration profileConfiguration)
        {
            Id = profileConfiguration.ProfileId;
            Name = profileConfiguration.Name;
            Order = profileConfiguration.Order;
            IsActive = profileConfiguration.Profile != null;
            IsSuspended = profileConfiguration.IsSuspended;
            IsMissingModule = profileConfiguration.IsMissingModule;
            HasActivationCondition = profileConfiguration.ActivationCondition != null;
            ActivationConditionMet = profileConfiguration.ActivationConditionMet;
            Category = new ProfileCategoryModel(profileConfiguration.Category);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsMissingModule { get; set; }
        public bool HasActivationCondition { get; set; }
        public bool ActivationConditionMet { get; set; }
        public ProfileCategoryModel Category { get; set; }
    }
}