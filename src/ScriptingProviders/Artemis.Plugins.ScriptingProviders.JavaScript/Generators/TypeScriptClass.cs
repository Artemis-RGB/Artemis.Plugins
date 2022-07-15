using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptClass
    {
        public const int MaxDepth = 4;

        public TypeScriptClass(ITypeScriptClassContainer? classContainer, Type type, bool includeMethods, int depth)
        {
            ClassContainer = classContainer;
            Type = type;
            IncludeMethods = includeMethods;

            Assembly assembly;
            DataModel? dataModel = null;
            if (classContainer is TypeScriptDataModel typeScriptDataModel)
            {
                assembly = typeScriptDataModel.DataModel.GetType().Assembly;
                dataModel = typeScriptDataModel.DataModel;
            }
            else if (classContainer != null)
                assembly = ((TypeScriptAssembly) classContainer).Assembly;
            else
                assembly = type.Assembly;

            if (Type.IsGenericType)
            {
                Type = type.GetGenericTypeDefinition();
                string stripped = Type.Name.Split('`')[0];
                Name = $"{stripped}<{string.Join(", ", ((TypeInfo) Type).GenericTypeParameters.Select(t => t.Name))}>";
            }
            else
                Name = Type.Name;

            foreach (PropertyInfo propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(t => t.MetadataToken)
                .GroupBy(prop => prop.Name)
                .Select(group => group.Aggregate((mostSpecific, other) => mostSpecific.DeclaringType.IsSubclassOf(other.DeclaringType) ? mostSpecific : other)))
            {
                if (propertyInfo.GetCustomAttribute<DataModelIgnoreAttribute>() != null)
                    continue;

                MethodInfo? getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null || getMethod.GetParameters().Any())
                    continue;

                // Skip properties that are in the ignored properties list of the respective profile module/data model expansion
                if (dataModel != null && dataModel.GetHiddenProperties().Any(p => p.Equals(propertyInfo)))
                    continue;

                Type propertyType = propertyInfo.PropertyType;

                // For primitives, simply create a property
                if (propertyType.IsPrimitive || propertyType.IsEnum || propertyType == typeof(string) || propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                    if (propertyType.IsEnum && !propertyType.IsGenericParameter && propertyType.GetEnumUnderlyingType() == typeof(int))
                        ClassContainer?.TypeScriptEnums.Add(new TypeScriptEnum(propertyType));
                }
                else if (Type.IsGenericType && propertyType.IsGenericParameter)
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                else if (propertyType.IsGenericEnumerable())
                {
                }
                else if (propertyType == typeof(DataModelEvent) || propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(DataModelEvent<>))
                {
                }
                // For other value types create a child view model
                else if (propertyType.IsClass || propertyType.IsStruct())
                {
                    TypeScriptProperties.Add(new TypeScriptProperty(propertyInfo));
                    if (depth <= MaxDepth && ClassContainer != null && ClassContainer.TypeScriptClasses.All(t => t.Type != propertyType) && propertyType.Assembly == assembly)
                        ClassContainer.TypeScriptClasses.Add(new TypeScriptClass(ClassContainer, propertyType, IncludeMethods, depth + 1));
                }
            }

            if (!includeMethods)
                return;

            foreach (MethodInfo methodInfo in Type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => !m.IsSpecialName && m.IsPrivate == false && m.DeclaringType?.Assembly == assembly && m.GetParameters().All(p => !p.IsIn && !p.IsOut)))
                TypeScriptMethods.Add(new TypeScriptMethod(methodInfo));
        }

        public ITypeScriptClassContainer? ClassContainer { get; }
        public Type Type { get; }
        public bool IncludeMethods { get; }
        public string Name { get; set; }
        public List<TypeScriptProperty> TypeScriptProperties { get; } = new();
        public List<TypeScriptMethod> TypeScriptMethods { get; } = new();

        public string GenerateCode(string prefix = "export", string? affix = null)
        {
            // Exclude anything that still came out invalid like nested generics which aren't supported
            string properties = string.Join("\r\n", TypeScriptProperties.Select(c => c.GenerateCode()).Where(c => !c.Contains("`")));
            string methods = string.Join("\r\n", TypeScriptMethods.Select(c => c.GenerateCode()).Where(c => !c.Contains("`")));
            return $"   {prefix} class {Name} {affix} {{\r\n" +
                   $"   {properties}\r\n" +
                   $"   {methods}\r\n" +
                   $"       {GenerateConstructor()}\r\n" +
                   "   }";
        }

        public string GetParameterType(Type type)
        {
            if (type.IsGenericType)
            {
                string stripped = type.Name.Split('`')[0];
                return $"{stripped}<{string.Join(", ", type.GenericTypeArguments.Select(TypeScriptProperty.GetTypeScriptType))}>";
            }

            return TypeScriptProperty.GetTypeScriptType(type);
        }

        private string GenerateConstructor()
        {
            ConstructorInfo[] constructors = Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            string result = "";
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                string parameters = string.Join(", ", constructorInfo.GetParameters().Select(param => param.Name + ": " + GetParameterType(param.ParameterType)));
                if (!parameters.Contains("`"))
                    result += $"constructor({parameters})\r\n";
            }

            return result == "" ? "private constructor()" : result;
        }
    }
}