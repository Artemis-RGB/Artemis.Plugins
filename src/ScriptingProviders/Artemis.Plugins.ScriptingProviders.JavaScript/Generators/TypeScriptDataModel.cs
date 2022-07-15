using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptDataModel : ITypeScriptClassContainer
    {
        public TypeScriptDataModel(DataModel dataModel, int index)
        {
            DataModel = dataModel;
            Name = $"DM{index}";
            TypeScriptClasses = new List<TypeScriptClass>();
            TypeScriptEnums = new List<TypeScriptEnum>();

            RootClass = new TypeScriptClass(this, dataModel.GetType(), false, 0);
            TypeScriptClasses.Add(RootClass);
        }

        public DataModel DataModel { get; set; }
        public string Name { get; set; }
        public TypeScriptClass RootClass { get; set; }

        public string GenerateCode()
        {
            string classes = string.Join("\r\n", TypeScriptClasses.GroupBy(c => c.Name).Select(g => g.First()).Select(c => c.GenerateCode("export", "extends Artemis.Core.DataModel")));
            string enums = string.Join("\r\n", TypeScriptEnums.GroupBy(c => c.Name).Select(g => g.First()).Select(c => c.GenerateCode()));
            return $"declare namespace {Name} {{\r\n" +
                   $"{classes}\r\n" +
                   $"{enums}\r\n" +
                   "}";
        }

        public List<TypeScriptClass> TypeScriptClasses { get; set; }
        public List<TypeScriptEnum> TypeScriptEnums { get; set; }
    }
}