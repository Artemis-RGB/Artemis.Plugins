using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Artemis.Core.Modules;
using Humanizer;
using Json.Schema;

namespace Artemis.Plugins.WebAPI.DataModels;

public class JsonSchemaDataModel : DataModel
{
    private readonly JsonSchema _schema;

    public JsonSchemaDataModel(JsonSchema schema)
    {
        _schema = schema;
        Load();
    }

    public void ApplyJsonObject(JsonObject jsonObject)
    {
        IReadOnlyDictionary<string, JsonSchema>? properties = _schema.GetProperties();
        if (properties == null)
            return;

        foreach ((string key, JsonNode? value) in jsonObject)
        {
            if (value == null)
                continue;

            JsonSchema? schema = properties.GetValueOrDefault(key);
            
            // Skip properties that are not defined in the schema
            if (schema == null)
                continue;

            if (schema.GetJsonType() == SchemaValueType.Array)
                ApplyJsonArray(schema, key, value);
            else if (schema.GetJsonType() == SchemaValueType.Object)
                GetDynamicChild<JsonSchemaDataModel>(key).Value.ApplyJsonObject(value.AsObject());
            else if (schema.GetJsonType() == SchemaValueType.Boolean)
                GetDynamicChild<bool>(key).Value = value.GetValue<bool>();
            else if (schema.GetJsonType() == SchemaValueType.String)
                GetDynamicChild<string>(key).Value = value.GetValue<string>();
            else if (schema.GetJsonType() == SchemaValueType.Number)
                GetDynamicChild<double>(key).Value = value.GetValue<double>();
            else if (schema.GetJsonType() == SchemaValueType.Integer)
                GetDynamicChild<int>(key).Value = value.GetValue<int>();
        }
    }

    private void Load()
    {
        IReadOnlyDictionary<string, JsonSchema>? properties = _schema.GetProperties();
        if (properties == null)
            return;

        foreach ((string key, JsonSchema value) in properties)
        {
            string title = value.GetTitle() ?? key.Humanize();
            string? description = value.GetDescription();
            if (value.GetJsonType() == SchemaValueType.Array)
                LoadArray(value, key, title, description);
            else if (value.GetJsonType() == SchemaValueType.Object)
                AddDynamicChild(key, new JsonSchemaDataModel(value), title, description);
            else if (value.GetJsonType() == SchemaValueType.Boolean)
                AddDynamicChild(key, false, title, description);
            else if (value.GetJsonType() == SchemaValueType.String)
                AddDynamicChild(key, "", title, description);
            else if (value.GetJsonType() == SchemaValueType.Number)
                AddDynamicChild(key, 0.0, title, description);
            else if (value.GetJsonType() == SchemaValueType.Integer)
                AddDynamicChild(key, 0, title, description);
            else if (value.GetJsonType() is SchemaValueType.Null or null)
            {
                // We'll skip these, they're not useful
            }
            else
                throw new NotSupportedException($"Unsupported JSON schema type {value.GetJsonType()}");
        }
    }

    private void LoadArray(JsonSchema value, string key, string title, string? description)
    {
        JsonSchema? items = value.GetItems();
        if (items != null)
        {
            if (items.GetJsonType() == SchemaValueType.Boolean)
                AddDynamicChild(key, new List<bool>(), title, description);
            else if (items.GetJsonType() == SchemaValueType.String)
                AddDynamicChild(key, new List<string>(), title, description);
            else if (items.GetJsonType() == SchemaValueType.Number)
                AddDynamicChild(key, new List<double>(), title, description);
            else if (items.GetJsonType() == SchemaValueType.Integer)
                AddDynamicChild(key, new List<int>(), title, description);
            else
                throw new NotSupportedException($"Unsupported JSON schema array type {items.GetJsonType()}");
        }
    }
    
    private void ApplyJsonArray(JsonSchema schema, string key, JsonNode value)
    {
        JsonSchema? items = schema.GetItems();
        if (items == null)
            return;
        if (items.GetJsonType() == SchemaValueType.Boolean)
            UpdateList(GetDynamicChild<List<bool>>(key).Value, value.AsArray());
        else if (items.GetJsonType() == SchemaValueType.String)
            UpdateList(GetDynamicChild<List<string>>(key).Value, value.AsArray());
        else if (items.GetJsonType() == SchemaValueType.Number)
            UpdateList(GetDynamicChild<List<double>>(key).Value, value.AsArray());
        else if (items.GetJsonType() == SchemaValueType.Integer)
            UpdateList(GetDynamicChild<List<int>>(key).Value, value.AsArray());
    }

    private static void UpdateList<T>(List<T> list, JsonArray array) where T : notnull
    {
        // Grow or shrink the list to match the array
        while (list.Count < array.Count)
            list.Add(default!);
        while (list.Count > array.Count)
            list.RemoveAt(list.Count - 1);

        int index = 0;
        IEnumerable<T> values = array.GetValues<T>();
        foreach (T arrayValue in values)
        {
            list[index] = arrayValue;
            index++;
        }
    }
}