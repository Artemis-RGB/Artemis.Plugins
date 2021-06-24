using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artemis.Core;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptAssembly : ITypeScriptClassContainer
    {
        public TypeScriptAssembly(string name, Assembly assembly)
        {
            Name = name;
            Assembly = assembly;

            TypeScriptClasses = new List<TypeScriptClass>();
            TypeScriptEnums = new List<TypeScriptEnum>();

            Type[] exportedTypes = assembly.GetExportedTypes();
            foreach (Type exportedType in exportedTypes.Where(t => t.IsClass || t.IsStruct()))
                TypeScriptClasses.Add(new TypeScriptClass(this, exportedType, true, 0));
        }

        public string Name { get; }
        public Assembly Assembly { get; }

        public string GenerateCode()
        {
            string classes = string.Join("\r\n", TypeScriptClasses.Where(c => c.Type.Assembly == Assembly).GroupBy(c => c.Name).Select(g => g.First()).Select(c => c.GenerateCode()));
            string enums = string.Join("\r\n", TypeScriptEnums.Where(c => c.EnumType.Assembly == Assembly).GroupBy(c => c.Name).Select(g => g.First()).Select(c => c.GenerateCode()));
            return $"declare namespace {Name} {{\r\n" +
                   $"{classes}\r\n" +
                   $"{enums}\r\n" +
                   "}";
        }

        public List<TypeScriptClass> TypeScriptClasses { get; }

        public List<TypeScriptEnum> TypeScriptEnums { get; }
    }

    public interface ITypeScriptClassContainer
    {
        List<TypeScriptClass> TypeScriptClasses { get; }
        List<TypeScriptEnum> TypeScriptEnums { get; }
    }
}