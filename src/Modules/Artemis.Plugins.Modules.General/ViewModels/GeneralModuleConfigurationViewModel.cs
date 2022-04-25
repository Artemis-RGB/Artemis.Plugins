using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.General.ViewModels
{
    public class GeneralModuleConfigurationViewModel : PluginConfigurationViewModel
    {
        public GeneralModuleConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            EnableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
            AutoDefaultProfilesCreation = settings.GetSetting("AutoDefaultProfilesCreation", true);

            EnableActiveWindow.AutoSave = true;
            AutoDefaultProfilesCreation.AutoSave = true;
        }

        public PluginSetting<bool> EnableActiveWindow { get; set; }
        public PluginSetting<bool> AutoDefaultProfilesCreation { get; set; }

        public override void OnCloseRequested()
        {
            EnableActiveWindow.AutoSave = false;
            AutoDefaultProfilesCreation.AutoSave = false;
        }
    }
}