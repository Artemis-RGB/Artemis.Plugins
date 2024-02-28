using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Artemis.Core;
using Artemis.Core.Modules;

namespace Artemis.Plugins.WebAPI.Json;

public class DataModelJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object)
        {
            jsonTypeInfo.Properties.Clear();
        
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.GetCustomAttribute<DataModelIgnoreAttribute>() != null)
                    continue;
                if (propertyInfo.PropertyType.IsAssignableTo(typeof(IDataModelEvent)))
                    continue;
        
                JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(propertyInfo.PropertyType, propertyInfo.Name);
                jsonPropertyInfo.Get = propertyInfo.CanRead ? propertyInfo.GetValue : null;
                jsonPropertyInfo.Set = propertyInfo.CanWrite ? (obj, value) => propertyInfo.SetValue(obj, value) : null;
                jsonTypeInfo.Properties.Add(jsonPropertyInfo);
            }
        }

        return jsonTypeInfo;
    }
}