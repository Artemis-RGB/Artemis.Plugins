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
        }

        public PluginSetting<bool> EnableActiveWindow { get; set; }
        public PluginSetting<bool> AutoDefaultProfilesCreation { get; set; }

        protected override void OnInitialActivate()
        {
            EnableActiveWindow.AutoSave = true;
            AutoDefaultProfilesCreation.AutoSave = true;
            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            EnableActiveWindow.AutoSave = false;
            AutoDefaultProfilesCreation.AutoSave = false;
            base.OnClose();
        }
    }
}