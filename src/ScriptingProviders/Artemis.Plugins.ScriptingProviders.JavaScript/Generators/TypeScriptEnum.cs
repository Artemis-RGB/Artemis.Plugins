using System;
using System.Linq;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Generators
{
    public class TypeScriptEnum
    {
        public TypeScriptEnum(Type enumType)
        {
            EnumType = enumType;
            Name = enumType.FullName?.Replace("+", "").Split(".").LastOrDefault() ?? enumType.Name;
            Values = (int[]) Enum.GetValues(enumType);
            Names = Enum.GetNames(enumType);
        }

        public Type EnumType { get; set; }

        public string Name { get; set; }
        public string[] Names { get; set; }
        public int[] Values { get; set; }

        public string GenerateCode()
        {
            string enums = "";
            for (int index = 0; index < Names.Length; index++)
                enums = enums + Names[index] + " = " + Values[index] + ",\r\n";

            return $"   enum {Name} {{\r\n" +
                   $"       {enums.Trim()}\r\n" +
                   "   }";
        }
    }
}