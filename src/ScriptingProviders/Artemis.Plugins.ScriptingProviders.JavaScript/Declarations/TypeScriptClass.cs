using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Declarations
{
    public class TypeScriptClass
    {
        private const int MaxDepth = 4;

        public TypeScriptClass(TypeScriptDataModel dataModel, Type type, int depth)
        {
            DataModel = dataModel;
            Type = type;

            if (Type.IsGenericType)
            {
                Type = type.GetGenericTypeDefinition();
                string stripped = Type.Name.Split('`')[0];
                Name = $"{stripped}<{string.Join(", ", ((TypeInfo)Type).GenericTypeParameters.Select(t => t.Name))}>";
            }
            else
            {
                Name = Type.Name;
            }

            TypeScriptProperties = new List<TypeScriptProperty>();

            foreach (PropertyInfo propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(t => t.MetadataToken)
                .GroupBy(prop => prop.Name)
                .Select(group => group.Aggregate((mostSpecificProp, other) => mostSpecificProp.DeclaringType.IsSubclassOf(other.DeclaringType) ? mostSpecificProp : other)))
            {
                if (propertyInfo.GetCustomAttribute<DataModelIgnoreAttribute>() != null)
                    continue;
                
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null || getMethod.GetParameters().Any())
                    continue;

                // Skip properties that are in the ignored properties list of the respective profile module/data model expansion
                if (DataModel.DataModel.GetHiddenProperties().Any(p => p.Equals(propertyInfo)))
                    continue;

                Type propertyType = propertyInfo.PropertyType;

                // For primitives, simply create a property
                if (propertyType.IsPrimitive || propertyType.IsEnum || propertyType == typeof(string) || propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                    if (propertyType.IsEnum)
                        DataModel.TypeScriptEnums.Add(new TypeScriptEnum(propertyType));
                }
                else if (Type.IsGenericType && propertyType.IsGenericParameter)
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                else if (propertyType.IsGenericEnumerable())
                    continue; // TODO
                else if (propertyType == typeof(DataModelEvent) || propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(DataModelEvent<>))
                    continue; // TODO
                // For other value types create a child view model
                else if (propertyType.IsClass || propertyType.IsStruct())
                {
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                    if (depth <= MaxDepth)
                        DataModel.TypeScriptClasses.Add(new TypeScriptClass(DataModel, propertyType, depth + 1));
                }
            }
        }

        public TypeScriptDataModel DataModel { get; }
        public Type Type { get; }

        public Type Source { get; set; }
        public string Name { get; set; }
        public List<TypeScriptProperty> TypeScriptProperties { get; set; }

        public string GenerateCode()
        {
            string properties = string.Join("\r\n", TypeScriptProperties.Select(c => c.GenerateCode()));
            return $@"
    export class {Name} {{
    {properties}
    private constructor()
}}";
        }
    }
}