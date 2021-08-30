using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.ServiceBindings
{
    public class DataModelBinding : IServiceBinding
    {
        private readonly IDataModelService _dataModelService;

        public DataModelBinding(IDataModelService dataModelService)
        {
            _dataModelService = dataModelService;
        }

        public void Initialize(Engine engine)
        {
            engine.Execute("const DataModel = {}");

            List<DataModel> list = _dataModelService.GetDataModels();
            for (int index = 0; index < list.Count; index++)
            {
                DataModel dataModel = list[index];
                string name;
                Type type = dataModel.GetType();
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                    string stripped = type.Name.Split('`')[0];
                    name = $"{stripped}<{string.Join(", ", ((TypeInfo) type).GenericTypeParameters.Select(t => t.Name))}>";
                }
                else
                {
                    name = type.Name;
                }

                string variableName = "DataModel" + Guid.NewGuid().ToString().Substring(0, 8);
                engine.SetValue(variableName, dataModel);
                engine.Execute($"DataModel.{name} = {variableName}");
            }
        }

        public string GetDeclaration()
        {
            TypeScriptDataModelCollection dataModelCollection = new(_dataModelService.GetDataModels());
            return dataModelCollection.GenerateCode();
        }
    }
}