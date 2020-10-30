using Artemis.Core;
using Artemis.Plugins.DataModelExpansions.TestData.DataModels;

namespace Artemis.Plugins.DataModelExpansions.TestData.ViewModels
{
    public class TestPluginConfigurationViewModel : PluginConfigurationViewModel
    {
        private string _event2Argument;

        public TestPluginConfigurationViewModel(PluginDataModelExpansion plugin) : base(plugin)
        {
            Plugin = plugin;
        }

        public new PluginDataModelExpansion Plugin { get; }

        public string Event2Argument
        {
            get => _event2Argument;
            set => SetAndNotify(ref _event2Argument, value);
        }

        public void TriggerEvent1()
        {
            Plugin.DataModel.Event1.Trigger();
        }

        public void TriggerEvent2()
        {
            Plugin.DataModel.Event2.Trigger(new TestEventArgs(Event2Argument));
        }
    }
}