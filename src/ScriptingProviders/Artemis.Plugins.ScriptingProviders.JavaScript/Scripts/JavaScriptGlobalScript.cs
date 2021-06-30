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

            Engine = plugin.Kernel!.Get<PluginJintEngine>(new ConstructorArgument("script", this));
            Engine.ExecuteScript();
        }

        public PluginJintEngine Engine { get; }

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