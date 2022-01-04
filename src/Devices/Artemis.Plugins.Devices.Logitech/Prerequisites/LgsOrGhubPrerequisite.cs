using Artemis.Core;
using System.Collections.Generic;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    internal class LgsOrGhubPrerequisite : PluginPrerequisite
    {
        private readonly Plugin _plugin;

        public LgsOrGhubPrerequisite(Plugin plugin)
        {
            _plugin = plugin;

            InstallActions = new()
            {
                new DownloadFileAction("Download LGHUB installer", "https://download01.logi.com/web/ftp/pub/techsupport/gaming/lghub_installer.exe", _plugin.ResolveRelativePath("lghub_installer.exe")),
                new ExecuteFileAction("Install LGHUB", _plugin.ResolveRelativePath("lghub_installer.exe"), null, true, true),
                new DeleteFileAction("Delete LGHUB installer", _plugin.ResolveRelativePath("lghub_installer.exe")),
            };

            UninstallActions = new();
        }

        public override string Name => "Required Logitech software";

        public override string Description => "Checks if the required Logitech software is installed on the system.";

        public override List<PluginPrerequisiteAction> InstallActions { get; }

        public override List<PluginPrerequisiteAction> UninstallActions { get; }

        public override bool IsMet()
        {
            return LogitechSoftwareChecker.IsLghubInstalled() || LogitechSoftwareChecker.IsLgsInstalled();
        }
    }
}