using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Artemis.Core;

namespace Artemis.Plugins.DataModelExpansions.TestData.Prerequisites
{
    public class TestPrerequisite1 : PluginPrerequisite
    {
        public TestPrerequisite1(Plugin plugin) : base(plugin)
        {
        }

        public override string Name => "Copy some large data";
        public override string Description => "The first test step";
        public override bool RequiresElevation => false;

        public override List<PluginPrerequisiteAction> InstallActions { get; } = new()
        {
            new CopyFolderAction("Test step", @"F:\Copy test", @"F:\Copy test 2")
        };

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new();

        public override Task<bool> IsMet()
        {
            return Task.Run(() => Directory.Exists(@"F:\Copy test 2"));
        }
    }

    public class TestPrerequisite2 : PluginPrerequisite
    {
        public TestPrerequisite2(Plugin plugin) : base(plugin)
        {
            InstallActions = new List<PluginPrerequisiteAction>()
            {
                new WriteToFileAction("Write data to file", @"F:\test.txt", Plugin.Directory.ToString())
            };
        }

        public override string Name => "Write data to a file";
        public override string Description => "The second test step";
        public override bool RequiresElevation => false;

        public override List<PluginPrerequisiteAction> InstallActions { get; } 

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new();

        public override Task<bool> IsMet()
        {
            return Task.Run(() => File.Exists(@"F:\test.txt"));
        }
    }
}