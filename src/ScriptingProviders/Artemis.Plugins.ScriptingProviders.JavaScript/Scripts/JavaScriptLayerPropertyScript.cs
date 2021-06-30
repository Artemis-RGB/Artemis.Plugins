using System;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptLayerPropertyScript : PropertyScript, IJavaScriptScript
    {
        private readonly LayerPropertyBinding _layerPropertyBinding;

        public JavaScriptLayerPropertyScript(ILayerProperty layerProperty, Plugin plugin, ScriptConfiguration configuration) : base(layerProperty, configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            Engine = plugin.Kernel!.Get<PluginJintEngine>(new ConstructorArgument("script", this));
            _layerPropertyBinding = new LayerPropertyBinding(layerProperty, plugin, Engine);

            Engine.ExtraValues.Add("LayerProperty", _layerPropertyBinding);
            Engine.ExecuteScript();
        }

        public PluginJintEngine Engine { get; }
        
        public override void OnPropertyUpdating(double deltaTime)
        {
            _layerPropertyBinding.LayerPropertyUpdating(deltaTime);
        }
        
        public override void OnPropertyUpdated(double deltaTime)
        {
            _layerPropertyBinding.LayerPropertyUpdated(deltaTime);
        }

        private void ConfigurationOnScriptContentChanged(object? sender, EventArgs e)
        {
            Engine.ExecuteScript();
        }

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ScriptConfiguration.ScriptContentChanged -= ConfigurationOnScriptContentChanged;
                Engine.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}