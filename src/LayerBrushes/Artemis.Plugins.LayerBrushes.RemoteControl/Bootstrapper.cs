using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.RemoteControl
{
    // This is your bootstrapper, you can do some kind of global setup work you need done here.
    // You can also just remove this file if you don't need it.
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            // Uncomment this to start using prerequisites
            // AddPluginPrerequisite(new MyPrerequisite());
            // AddFeaturePrerequisite<PluginModule>(new MyPrerequisite());
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }

    // Uncomment this to start using prerequisites or create your own from scratch
    // public class MyPrerequisite : PluginPrerequisite
    // {
    //     #region Overrides of PluginPrerequisite
    //
    //     /// <inheritdoc />
    //     public override bool IsMet()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     /// <inheritdoc />
    //     public override string Name { get; }
    //
    //     /// <inheritdoc />
    //     public override string Description { get; }
    //
    //     /// <inheritdoc />
    //     public override bool RequiresElevation { get; }
    //
    //     /// <inheritdoc />
    //     public override List<PluginPrerequisiteAction> InstallActions { get; }
    //
    //     /// <inheritdoc />
    //     public override List<PluginPrerequisiteAction> UninstallActions { get; }
    //
    //     #endregion
    // }
}