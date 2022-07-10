using System.Collections.Generic;
using System.IO;
using Artemis.Core;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Prerequisites.Linux;

public class BassPrerequisite : PluginPrerequisite
{
    public BassPrerequisite(Plugin plugin)
    {
        Platform = PluginPlatform.Linux;
        InstallActions = new List<PluginPrerequisiteAction>()
        {
            new ExtractArchiveAction("BASS", plugin.ResolveRelativePath(Path.Combine("Resources", "bass.zip")), Constants.ApplicationFolder) {FilesToExtract = new List<string> {"libbass.so"}}
        };
        UninstallActions = new List<PluginPrerequisiteAction>
        {
            new DeleteFileAction("BASS", Path.Combine(Constants.ApplicationFolder, "libbass.so"))
        };
    }

    public override bool IsMet()
    {
        return File.Exists(Path.Combine(Constants.ApplicationFolder, "libbass.so"));
    }

    public override string Name => "BASS Audio Library";
    public override string Description => "Allows scripts to play sounds";
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }
}