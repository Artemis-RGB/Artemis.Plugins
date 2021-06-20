using Artemis.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Plugins.ScriptingProviders.JavaScript.Prerequisites;

namespace Artemis.Plugins.ScriptingProviders.JavaScript
{
    // This is your bootstrapper, you can do some kind of global setup work you need done here.
    // You can also just remove this file if you don't need it.
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            // Uncomment this to start using prerequisites
            AddPluginPrerequisite(new WebView2Runtime(plugin));
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }

    // Uncomment this to start using prerequisites or create your own from scratch
}