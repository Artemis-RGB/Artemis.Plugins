using System;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptGlobalScript : GlobalScript, IJavaScriptScript
    {
        public JavaScriptGlobalScript(Plugin plugin, ScriptConfiguration configuration) : base(configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            EngineManager = plugin.Kernel!.Get<EngineManager>(new ConstructorArgument("script", this));
            EngineManager.ExecuteScript();
        }

        public EngineManager EngineManager { get; }

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