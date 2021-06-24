using System;
using System.Linq;
using System.Reflection;
using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptProperty
    {
        public TypeScriptProperty(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            DataModelPropertyAttribute dataModelPropertyAttribute = propertyInfo.GetCustomAttribute<DataModelPropertyAttribute>();

            Name = propertyInfo.Name;
            Comment = dataModelPropertyAttribute?.Description;

            Type propertyType = propertyInfo.PropertyType;
            if (!propertyType.IsGenericType)
                Type = GetTypeScriptType(propertyType);
            else
            {
                Type genericTypeDefinition = propertyType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    Type = GetTypeScriptType(propertyType.GenericTypeArguments[0]);
                    Name += "?";
                }

                else
                {
                    string stripped = genericTypeDefinition.Name.Split('`')[0];
                    Type = $"{stripped}<{string.Join(", ", propertyType.GenericTypeArguments.Select(t => GetTypeScriptType(t)))}>";
                }
            }
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public PropertyInfo PropertyInfo { get; }

        public string GenerateCode()
        {
            string prefix = PropertyInfo.GetSetMethod() != null ? "" : "readonly ";
            if (string.IsNullOrWhiteSpace(Comment))
                return $"{prefix}{Name}: {Type}";

            return "/**\r\n" +
                   $"* {Comment}\r\n" +
                   "*/\r\n" +
                   $"{prefix}{Name}: {Type}";
        }


        public static string GetTypeScriptType(Type type)
        {
            if (type.TypeIsNumber())
                return "number";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(string))
                return "string";
            if (type == typeof(void))
                return "void";
            return type.FullName?.Replace("+", "") ?? type.Name;
        }
    }
}