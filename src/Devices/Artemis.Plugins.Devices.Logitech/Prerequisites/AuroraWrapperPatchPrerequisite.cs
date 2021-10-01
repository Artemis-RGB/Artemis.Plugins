using Artemis.Core;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    /// <summary>
    /// Checks for presence of Aurora's Wrapper patch dll in the system directory
    /// </summary>
    internal class AuroraWrapperPatchPrerequisite : PluginPrerequisite
    {
        public override string Name => "Logitech Dll Check";

        public override string Description => "Checks for presence of the backup logitech dll placed by Aurora";

        public override List<PluginPrerequisiteAction> InstallActions { get; } = new()
        {
            new RunInlinePowerShellAction("Copy backup dll over", $"Move-Item -Path \"{AURORA_BACKUP_PATH}\" -Destination \"{AURORA_WRAPPER_PATH}\"" , true)
        };

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new();

        public override bool IsMet() => !File.Exists(AURORA_BACKUP_PATH);

        private const string AURORA_WRAPPER_PATH = "C:\\Program Files\\Logitech Gaming Software\\SDK\\LED\\x64\\LogitechLed.dll";
        private const string AURORA_BACKUP_PATH = "C:\\Program Files\\Logitech Gaming Software\\SDK\\LED\\x64\\LogitechLed.dll.aurora_backup";
    }
}