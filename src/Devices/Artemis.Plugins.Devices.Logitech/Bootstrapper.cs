using Artemis.Core;
using Artemis.Plugins.Devices.Logitech.Prerequisites;

namespace Artemis.Plugins.Devices.Logitech
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            AddPluginPrerequisite(new VcRedistPrerequisite(plugin));
            AddPluginPrerequisite(new LgsOrGhubPrerequisite(plugin));
            AddPluginPrerequisite(new AuroraWrapperPatchPrerequisite());
        }
    }
}