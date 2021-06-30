using System;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Ninject;
using Ninject.Parameters;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptLayerScript : LayerScript, IJavaScriptScript
    {
        private readonly LayerBinding _layerBinding;

        public JavaScriptLayerScript(Layer layer, Plugin plugin, ScriptConfiguration configuration) : base(layer, configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            Engine = plugin.Kernel!.Get<PluginJintEngine>(new ConstructorArgument("script", this));
            _layerBinding = new LayerBinding(layer, plugin, Engine);

            Engine.ExtraValues.Add("Layer", _layerBinding);
            Engine.ExecuteScript();
        }

        public PluginJintEngine Engine { get; }
        
        public override void OnLayerUpdating(double deltaTime)
        {
            _layerBinding.LayerUpdating(deltaTime);
        }
        
        public override void OnLayerUpdated(double deltaTime)
        {
            _layerBinding.LayerUpdated(deltaTime);
        }
        
        public override void OnLayerRendering(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            _layerBinding.LayerRendering(canvas, bounds, paint);
        }
        
        public override void OnLayerRendered(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            _layerBinding.LayerRendered(canvas, bounds, paint);
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