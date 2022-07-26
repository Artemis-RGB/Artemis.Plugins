using System.Collections.Generic;
using System.IO;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Serilog;

namespace Artemis.Plugins.Modules.DefaultProfile
{
    [PluginFeature(Name = "Default profile", Icon = "ToyBrickPlus", AlwaysEnabled = true)]
    public class DefaultProfileModule : Module
    {
        private readonly IProfileService _profileService;
        private readonly ILogger _logger;
        private readonly string[] _defaultProfileFilepaths = new string[]
        {
            "Profiles/rainbow.json",
            "Profiles/noise.json"
        };

        public override List<IModuleActivationRequirement> ActivationRequirements => null;

        public DefaultProfileModule(IProfileService profileService, ILogger logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        public override void Update(double deltaTime) { }

        public override void Enable()
        {
            // Validate if there are profiles
            if (_profileService.ProfileConfigurations.Count == 0)
            {
                _logger.Information("There are no created profiles. Procedding. Proceeding to create default profiles");
                foreach (string profileFilePath in _defaultProfileFilepaths)
                {
                    if (File.Exists(profileFilePath))
                    {
                        if (AddDefaultProfile(DefaultCategoryName.General, profileFilePath))
                            _logger.Information($"Default profile file {profileFilePath} imported successfully");
                    }
                    else
                    {
                        _logger.Warning($"Default profile file {profileFilePath} don't exists. Skipping profile creation");
                    }
                }
            }
            _logger.Information("There are one or more profiles. No new default profiles will be created");
        }

        public override void Disable() { }
    }
}