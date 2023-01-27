using System;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptGlobalScript : GlobalScript, IJavaScriptScript
    {
        public JavaScriptGlobalScript(ScriptConfiguration configuration, Func<Script, EngineManager> createEngineManager) : base(configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            EngineManager = createEngineManager(this);
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