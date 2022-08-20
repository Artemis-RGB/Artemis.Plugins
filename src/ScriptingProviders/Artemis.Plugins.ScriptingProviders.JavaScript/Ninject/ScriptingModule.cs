using Artemis.Plugins.ScriptingProviders.JavaScript.Bindings;
using Artemis.Plugins.ScriptingProviders.JavaScript.Services;
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

            Kernel!.Bind<ScriptEditorService>().ToSelf().InSingletonScope();
        }

        #endregion
    }
}