using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.General.ViewModels
{
    public class GeneralModuleConfigurationViewModel : PluginConfigurationViewModel
    {
        public GeneralModuleConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            EnableActiveWindow = settings.GetSetting("EnableActiveWindow", true);
        }

        public PluginSetting<bool> EnableActiveWindow { get; set; }

        protected override void OnInitialActivate()
        {
            EnableActiveWindow.AutoSave = true;
            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            EnableActiveWindow.AutoSave = false;
            base.OnClose();
        }
    }
}