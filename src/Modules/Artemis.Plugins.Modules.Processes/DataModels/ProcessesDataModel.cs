using System.Collections.Generic;
using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.Processes.DataModels
{
    public class ProcessesDataModel : DataModel
    {
        public WindowDataModel ActiveWindow { get; set; }

        [DataModelProperty(ListItemName = "Process name")]
        public List<string> RunningProcesses { get; set; }
    }
}