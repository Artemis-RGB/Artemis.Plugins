using System;
using System.Linq;
using System.Reflection;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptMethod
    {
        public TypeScriptMethod(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }

        public MethodInfo MethodInfo { get; }

        public string GenerateCode()
        {
            string prefix = "";
            if (MethodInfo.IsStatic)
                prefix = "static ";

            string parameters = string.Join(", ", MethodInfo.GetParameters().Select(param => param.Name + ": " + GetParameterType(param.ParameterType)));
            string method = $"{prefix}{MethodInfo.Name}({parameters}): {GetParameterType(MethodInfo.ReturnType)}\r\n";
            return method;
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
    }
}