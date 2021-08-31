using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Models;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class ProfilesController : WebApiController
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Route(HttpVerbs.Get, "/profiles/categories")]
        public IEnumerable<ProfileCategoryModel> GetProfileCategories()
        {
            return _profileService.ProfileCategories.Select(c => new ProfileCategoryModel(c));
        }

        [Route(HttpVerbs.Get, "/profiles")]
        public IEnumerable<ProfileConfigurationModel> GetProfileConfigurations()
        {
            return _profileService.ProfileConfigurations.Select(c => new ProfileConfigurationModel(c));
        }

        [Route(HttpVerbs.Post, "/profiles/suspend/{profileId}")]
        public void GetProfileConfigurations(Guid profileId, [FormField] bool suspend)
        {
            ProfileConfiguration profileConfiguration = _profileService.ProfileConfigurations.FirstOrDefault(p => p.ProfileId == profileId);
            if (profileConfiguration == null)
                throw HttpException.NotFound("Profile configuration not found.");

            profileConfiguration.IsSuspended = suspend;
            _profileService.SaveProfileCategory(profileConfiguration.Category);
        }
    }
}