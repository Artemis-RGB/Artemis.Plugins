using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.General.ViewModels
{
    public class GeneralModuleConfigurationViewModel : PluginConfigurationViewModel
    {
        public GeneralModuleConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            EnableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
            DisableDefaultProfilesCreation = settings.GetSetting("DisableDefaultProfilesCreation", true);
        }

        public PluginSetting<bool> EnableActiveWindow { get; set; }
        public PluginSetting<bool> DisableDefaultProfilesCreation { get; set; }

        protected override void OnInitialActivate()
        {
            EnableActiveWindow.AutoSave = true;
            DisableDefaultProfilesCreation.AutoSave = true;
            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            EnableActiveWindow.AutoSave = false;
            DisableDefaultProfilesCreation.AutoSave = false;
            base.OnClose();
        }
    }
}