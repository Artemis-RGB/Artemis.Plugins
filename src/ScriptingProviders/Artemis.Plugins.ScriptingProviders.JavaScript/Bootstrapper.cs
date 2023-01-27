using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using DryIoc;

namespace Artemis.Plugins.ScriptingProviders.JavaScript
{
    public class Bootstrapper : PluginBootstrapper
    {
        private bool _loadedModule;

        public override void OnPluginLoaded(Plugin plugin)
        {
            AddFeaturePrerequisite<JavaScriptScriptingProvider>(new Prerequisites.Windows.BassPrerequisite(plugin));
            AddFeaturePrerequisite<JavaScriptScriptingProvider>(new Prerequisites.Linux.BassPrerequisite(plugin));
            AddFeaturePrerequisite<JavaScriptScriptingProvider>(new Prerequisites.OSX.BassPrerequisite(plugin));
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
            if (!_loadedModule)
            {
                plugin.Container.RegisterMany(new []{plugin.Assembly}, type => type.IsAssignableTo<IScriptBinding>());
                _loadedModule = true;
            }
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}