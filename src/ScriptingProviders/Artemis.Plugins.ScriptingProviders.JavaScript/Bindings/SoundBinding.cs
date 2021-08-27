using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public class SoundBinding : IScriptBinding
    {
        public SoundBinding(PluginJintEngine engine)
        {
            
        }
        public string Name => "sound";
    }
}
