using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.TestData.DataModels
{
    public class DynamicDataModel : DataModel
    {
        public DynamicDataModel()
        {
            DynamicString = "Test 123";
        }

        [DataModelProperty(Description = "Descriptionnnnnn")]
        public string DynamicString { get; set; }
    }
}