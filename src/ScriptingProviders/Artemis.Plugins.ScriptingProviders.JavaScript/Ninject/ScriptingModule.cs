using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Ninject
{
    public class ScriptingModule : NinjectModule
    {
        #region Overrides of NinjectModule

        /// <inheritdoc />
        public override void Load()
        {
            Kernel.Bind(x =>
            {
                x.FromThisAssembly()
                    .SelectAllClasses()
                    .InheritedFrom<IScriptBinding>()
                    .BindAllInterfaces();
            });
        }

        #endregion
    }
}