using Artemis.Core;
using Artemis.Plugins.Modules.TestData.DataModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.TestData.ViewModels
{
    public class TestPluginConfigurationViewModel : PluginConfigurationViewModel
    {
        private string _event2Argument;

        public TestPluginConfigurationViewModel(Plugin plugin) : base(plugin)
        {
            DataModelExpansion = Plugin.GetFeature<TestModule>();
        }

        public TestModule DataModelExpansion { get; }
        public bool CanTriggerEvent1 => DataModelExpansion != null;
        public bool CanTriggerEvent2 => DataModelExpansion != null;

        public string Event2Argument
        {
            get => _event2Argument;
            set => SetAndNotify(ref _event2Argument, value);
        }

        public void TriggerEvent1()
        {
            DataModelExpansion?.DataModel.Event1.Trigger();
        }

        public void TriggerEvent2()
        {
            DataModelExpansion?.DataModel.Event2.Trigger(new TestEventArgs(Event2Argument));
        }
    }
}