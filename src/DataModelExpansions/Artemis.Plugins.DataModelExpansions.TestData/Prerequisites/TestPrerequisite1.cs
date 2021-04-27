using System.Collections.Generic;
using System.IO;
using Artemis.Core;

namespace Artemis.Plugins.DataModelExpansions.TestData.Prerequisites
{
    public class TestPrerequisite1 : PluginPrerequisite
    {
        public TestPrerequisite1(Plugin plugin) : base(plugin)
        {
        }

        public override string Name => "First prerequisite";
        public override string Description => "This is number one!";
        public override bool RequiresElevation => false;

        public override List<PluginPrerequisiteAction> InstallActions { get; } = new()
        {
            new CopyFolderAction("Copy big folder", @"F:\Copy test", @"F:\Copy test 2")
        };

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new()
        {
            new DeleteFolderAction("Remove big folder", @"F:\Copy test 2")
        };

        public override bool IsMet()
        {
            return Directory.Exists(@"F:\Copy test 2");
        }
    }
}