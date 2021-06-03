using System.Collections.Generic;
using System.IO;
using Artemis.Core;
using Microsoft.Win32;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    public class VcRedistPrerequisite : PluginPrerequisite
    {
        /// <inheritdoc />
        public VcRedistPrerequisite(Plugin plugin)
        {
            string installerPath = Path.Combine(plugin.Directory.FullName, "vcredist_x64.exe");

            InstallActions = new List<PluginPrerequisiteAction>
            {
                new DownloadFileAction("Download installer", "https://aka.ms/highdpimfc2013x64enu", installerPath),
                new ExecuteFileAction("Run installer", installerPath, "-passive"),
                new DeleteFileAction("Clean up", installerPath)
            };
        }

        #region Overrides of PluginPrerequisite

        /// <inheritdoc />
        public override bool IsMet()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\12.0\VC\Runtimes\x64", false);
            string majorValue = key?.GetValue("Major")?.ToString();
            if (majorValue == null)
                return false;

            return int.Parse(majorValue) >= 12;
        }

        /// <inheritdoc />
        public override string Name => "Visual C++ Redistributable Packages 2013";

        /// <inheritdoc />
        public override string Description => "Required by the Logitech SDK";
        
        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> InstallActions { get; }

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new();

        #endregion
    }
}