using System.Collections.Generic;
using Artemis.Core.DataModelExpansions;

namespace Artemis.Plugins.PhilipsHue.DataModels
{
    public class PluginDataModel : DataModel
    {
        public PluginDataModel()
        {
            Groups = new List<GroupDataModel>();
        }

        [DataModelIgnore]
        public List<GroupDataModel> Groups { get; set; }
    }
}