using Artemis.Core;
using Artemis.Core.ScriptingProviders;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptLayerPropertyScript : PropertyScript
    {
        public JavaScriptLayerPropertyScript(ILayerProperty layerProperty, ScriptConfiguration configuration) : base(layerProperty, configuration)
        {
        }
    }
}