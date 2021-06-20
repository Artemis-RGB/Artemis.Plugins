using System.Collections.Generic;
using System.IO;
using Artemis.Core;
using Microsoft.Win32;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Prerequisites
{
    public class WebView2Runtime : PluginPrerequisite
    {
        public WebView2Runtime(Plugin plugin)
        {
            string installerPath = Path.Combine(plugin.Directory.FullName, "MicrosoftEdgeWebview2Setup.exe");

            InstallActions = new List<PluginPrerequisiteAction>
            {
                new DownloadFileAction("Download installer", "https://go.microsoft.com/fwlink/p/?LinkId=2124703", installerPath),
                new ExecuteFileAction("Run installer", installerPath, "/silent /install", true, true),
                new DeleteFileAction("Clean up", installerPath)
            };
        }

        /// <inheritdoc />
        public override bool IsMet()
        {
            return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", false) != null;
        }

        /// <inheritdoc />
        public override string Name => "WebView 2 Runtime";

        /// <inheritdoc />
        public override string Description => "The runtime required to use the code editor";
        
        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> InstallActions { get; }

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> UninstallActions { get; }

     
    }
}