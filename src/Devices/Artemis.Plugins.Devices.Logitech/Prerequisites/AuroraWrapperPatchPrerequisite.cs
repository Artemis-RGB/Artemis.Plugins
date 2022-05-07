using Artemis.Core;
using System.Collections.Generic;
using System.IO;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    /// <summary>
    /// Checks for presence of Aurora's Wrapper patch dll in the system directory
    /// </summary>
    internal class AuroraWrapperPatchPrerequisite : PluginPrerequisite
    {
        public override string Name => "Logitech DLL Check";

        public override string Description => "Checks for presence of the backup Logitech DLL placed by Aurora";

        public override List<PluginPrerequisiteAction> InstallActions { get; } = new()
        {
            new RunInlinePowerShellAction("Delete Aurora Wrapper DLL", $"Remove-Item -Path \"{AURORA_WRAPPER_PATH}\" -Force", true),
            new RunInlinePowerShellAction("Restore original backed up DLL", $"Move-Item -Path \"{AURORA_BACKUP_PATH}\" -Destination \"{AURORA_WRAPPER_PATH}\"", true)
        };

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new();

        public override bool IsMet()
        {
            if (LogitechSoftwareChecker.GetLogitechDllPath() == AURORA_WRAPPER_PATH)
                return !File.Exists(AURORA_BACKUP_PATH);

            return true;
        }

        private const string AURORA_WRAPPER_PATH = @"C:\Program Files\Logitech Gaming Software\SDK\LED\x64\LogitechLed.dll";
        private const string AURORA_BACKUP_PATH = @"C:\Program Files\Logitech Gaming Software\SDK\LED\x64\LogitechLed.dll.aurora_backup";
    }
}