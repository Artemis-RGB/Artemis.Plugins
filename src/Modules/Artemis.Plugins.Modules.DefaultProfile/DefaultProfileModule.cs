using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Serilog;

namespace Artemis.Plugins.Modules.DefaultProfile;

[PluginFeature(Name = "Default profile", AlwaysEnabled = true)]
public class DefaultProfileModule : Module
{
    private readonly string[] _defaultProfileFilepaths =
    {
        "./Profiles/Rainbow.zip",
        "./Profiles/Noise.zip"
    };

    private readonly ILogger _logger;
    private readonly Plugin _plugin;
    private readonly IProfileService _profileService;

    public DefaultProfileModule(Plugin plugin, IProfileService profileService, ILogger logger)
    {
        _plugin = plugin;
        _profileService = profileService;
        _logger = logger;
    }

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    public override void Update(double deltaTime)
    {
    }

    public override void Enable()
    {
        // Validate if there are profiles
        if (!_profileService.ProfileCategories.SelectMany(c => c.ProfileConfigurations).Any())
        {
            _logger.Information("There are no created profiles. Proceeding to create default profiles");
            foreach (string profileFilePath in _defaultProfileFilepaths)
            {
                string resolvedPath = _plugin.ResolveRelativePath(profileFilePath);
                if (File.Exists(resolvedPath))
                {
                    if (AddDefaultProfile(DefaultCategoryName.General, profileFilePath))
                        _logger.Information("Default profile file {ResolvedPath} imported successfully", resolvedPath);
                }
                else
                {
                    _logger.Warning("Default profile file {ResolvedPath} doesn't exists. Skipping profile creation", resolvedPath);
                }
            }
        }

        _logger.Information("There are one or more profiles. No new default profiles will be created");
    }

    public override void Disable()
    {
    }
}