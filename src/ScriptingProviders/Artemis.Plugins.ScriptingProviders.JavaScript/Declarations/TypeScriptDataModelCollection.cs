using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Declarations
{
    public class TypeScriptDataModelCollection
    {
        public TypeScriptDataModelCollection(List<DataModel> dataModels)
        {
            NameSpaces = new List<TypeScriptDataModel>();
            for (int index = 0; index < dataModels.Count; index++)
                NameSpaces.Add(new TypeScriptDataModel(dataModels[index], index));
        }

        public List<TypeScriptDataModel> NameSpaces { get; set; }

        public string GenerateCode()
        {
            string namespaces = string.Join("\r\n", NameSpaces.Select(n => n.GenerateCode()));
            string rootClasses = string.Join("\r\n", NameSpaces.Select(n => $"  readonly {n.RootClass.Name}: {n.Name}.{n.RootClass.Name}"));
            string code = $@"
{namespaces}
declare class DataModelContainer {{
{rootClasses}
}}
const dataModel = new DataModelContainer();";
            return code;
        }
    }
}