using System;
using Artemis.Core;

namespace Artemis.Plugins.WebAPI.Models
{
    public class ProfileCategoryModel
    {
        public ProfileCategoryModel(ProfileCategory category)
        {
            Id = category.EntityId;
            Name = category.Name;
            Order = category.Order;
            IsSuspended = category.IsSuspended;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsSuspended { get; set; }
    }
}