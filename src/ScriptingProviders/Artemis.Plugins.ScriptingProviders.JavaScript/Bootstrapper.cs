using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Ninject;
using Artemis.Plugins.ScriptingProviders.JavaScript.Prerequisites;

namespace Artemis.Plugins.ScriptingProviders.JavaScript
{
    public class Bootstrapper : PluginBootstrapper
    {
        private bool _loadedModule;

        public override void OnPluginLoaded(Plugin plugin)
        {
            AddPluginPrerequisite(new WebView2Runtime(plugin));
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
            if (!_loadedModule)
            {
                plugin.Kernel.Load(new[] {new ScriptingModule()});
                _loadedModule = true;
            }
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}