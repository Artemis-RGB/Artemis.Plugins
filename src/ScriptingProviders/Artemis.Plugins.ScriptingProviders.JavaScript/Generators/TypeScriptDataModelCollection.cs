using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptDataModelCollection
    {
        public TypeScriptDataModelCollection(List<DataModel> dataModels)
        {
            DataModels = new List<TypeScriptDataModel>();
            for (int index = 0; index < dataModels.Count; index++)
                DataModels.Add(new TypeScriptDataModel(dataModels[index], index));
        }

        public List<TypeScriptDataModel> DataModels { get; set; }

        public string GenerateCode()
        {
            string dataModels = string.Join("\r\n", DataModels.Select(n => n.GenerateCode()));
            string rootClasses = string.Join("\r\n", DataModels.Select(n => $"  readonly {n.RootClass.Name}: {n.Name}.{n.RootClass.Name}"));
            string code = $"{dataModels}\r\n" +
                          "declare class DataModelContainer {\r\n" +
                          $"{rootClasses}\r\n" +
                          "}\r\n" +
                          "const dataModel = new DataModelContainer();";
            return code;
        }
    }
}