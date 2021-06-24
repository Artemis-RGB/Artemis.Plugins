using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings
{
    public class DataModelBinding : IScriptBinding
    {
        public DataModelBinding(PluginJintEngine engine, IDataModelService dataModelService)
        {
            engine.Engine.Execute("const dataModel = {}");

            List<DataModel> list = dataModelService.GetDataModels();
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
                    name = type.Name;

                string variableName = "dataModel" + Guid.NewGuid().ToString().Substring(0, 8);
                engine.Engine.SetValue(variableName, dataModel);
                engine.Engine.Execute($"dataModel.{name} = {variableName}");
            }
        }

        #region Implementation of IScriptBinding

        /// <inheritdoc />
        public string Name => "dataModelService";

        #endregion
    }
}