using System.Collections.Generic;
using System.IO;
using Artemis.Core;

namespace Artemis.Plugins.DataModelExpansions.TestData.Prerequisites
{
    public class TestPrerequisite2 : PluginPrerequisite
    {
        public TestPrerequisite2()
        {
            InstallActions = new List<PluginPrerequisiteAction>
            {
                new WriteStringToFileAction("Write data to file", @"F:\test2.txt", "Test 123") {Append = true}
            };
        }

        public override string Name => "Second prerequisite";
        public override string Description => "This is number two!";
        public override bool RequiresElevation => false;

        public override List<PluginPrerequisiteAction> InstallActions { get; }

        public override List<PluginPrerequisiteAction> UninstallActions { get; } = new()
        {
            new DeleteFileAction("Remove file", @"F:\test2.txt")
        };

        public override bool IsMet()
        {
            return File.Exists(@"F:\test2.txt");
        }
    }
}