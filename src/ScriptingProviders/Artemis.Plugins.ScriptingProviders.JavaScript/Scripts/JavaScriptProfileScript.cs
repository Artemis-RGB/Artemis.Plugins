using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.Manual;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Ninject;
using Ninject.Parameters;
using SkiaSharp;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptProfileScript : ProfileScript
    {
        private readonly PluginJintEngine _engine;
        private ProfileBinding _profileBinding;

        public JavaScriptProfileScript(Profile profile, Plugin plugin, ScriptConfiguration configuration) : base(profile, configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            _engine = plugin.Kernel!.Get<PluginJintEngine>(new ConstructorArgument("script", this));
            _profileBinding = new ProfileBinding(profile, _engine);

            _engine.ExtraValues.Add("profile", _profileBinding);
            _engine.ExecuteScript();
        }

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

        private void ConfigurationOnScriptContentChanged(object sender, EventArgs e)
        {
            _engine.ExecuteScript();
        }

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ScriptConfiguration.ScriptContentChanged -= ConfigurationOnScriptContentChanged;
                _engine.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}