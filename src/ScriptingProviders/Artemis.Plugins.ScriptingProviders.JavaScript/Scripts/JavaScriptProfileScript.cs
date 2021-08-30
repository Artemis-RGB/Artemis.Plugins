using System;
using System.IO;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.ContextBindings;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Ninject;
using Ninject.Parameters;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptProfileScript : ProfileScript, IJavaScriptScript
    {
        private readonly ProfileBinding _profileBinding;

        public JavaScriptProfileScript(Profile profile, Plugin plugin, ScriptConfiguration configuration) : base(profile, configuration)
        {
            _profileBinding = new ProfileBinding(profile, plugin);

            EngineManager = plugin.Kernel!.Get<EngineManager>(new ConstructorArgument("script", this));
            EngineManager.ContextBindings.Add(_profileBinding);
            EngineManager.ExecuteScript();

            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;
        }

        public EngineManager EngineManager { get; }

        public override void OnProfileUpdating(double deltaTime)
        {
            _profileBinding.ProfileUpdating(deltaTime);
        }

        public override void OnProfileUpdated(double deltaTime)
        {
            _profileBinding.ProfileUpdated(deltaTime);
        }

        public override void OnProfileRendering(SKCanvas canvas, SKRect bounds)
        {
            _profileBinding.ProfileRendering(canvas, bounds);
        }

        public override void OnProfileRendered(SKCanvas canvas, SKRect bounds)
        {
            _profileBinding.ProfileRendered(canvas, bounds);
        }

        private void ConfigurationOnScriptContentChanged(object? sender, EventArgs e)
        {
            EngineManager.ExecuteScript();
        }

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ScriptConfiguration.ScriptContentChanged -= ConfigurationOnScriptContentChanged;
                EngineManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}