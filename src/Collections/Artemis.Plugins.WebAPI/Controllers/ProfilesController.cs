using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.Models;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Webservices;

namespace Artemis.Plugins.WebAPI.Controllers
{
    internal class ProfilesController
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [ResourceMethod]
        public IEnumerable<ProfileCategoryModel> GetProfileCategories()
        {
            return _profileService.ProfileCategories.Select(c => new ProfileCategoryModel(c));
        }

        [ResourceMethod]
        public IEnumerable<ProfileConfigurationModel> GetProfileConfigurations()
        {
            return _profileService.ProfileCategories.SelectMany(c => c.ProfileConfigurations).Select(c => new ProfileConfigurationModel(c));
        }

        [ResourceMethod(RequestMethod.Post, "suspend/:profileId")]
        public IResponseBuilder SuspendProfile(IRequest request, Guid profileId, bool suspend)
        {
            ProfileConfiguration? profileConfiguration = _profileService.ProfileCategories
                .SelectMany(c => c.ProfileConfigurations)
                .FirstOrDefault(p => p.ProfileId == profileId);

            if (profileConfiguration == null)
                return request.Respond().Status(ResponseStatus.NotFound);

            profileConfiguration.IsSuspended = suspend;
            _profileService.SaveProfileCategory(profileConfiguration.Category);

            return request.Respond().Status(ResponseStatus.NoContent);
        }
    }
}