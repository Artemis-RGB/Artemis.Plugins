using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.WebAPI.DataModels;
using Artemis.Plugins.WebAPI.Features;
using Json.Schema;
using Serilog;

namespace Artemis.Plugins.WebAPI.Services;

public class JsonModuleService : IJsonModuleService
{
    private readonly ILogger _logger;
    private readonly Plugin _plugin;
    private readonly PluginSetting<Dictionary<string, JsonModule>> _store;
    private readonly Dictionary<string, JsonModule> _jsonModules;
    private readonly Dictionary<JsonModule, JsonSchemaDataModel> _dataModels = [];

    public JsonModuleService(ILogger logger, PluginSettings pluginSettings, Plugin plugin)
    {
        _logger = logger;
        _plugin = plugin;
        _store = pluginSettings.GetSetting("JsonModules", new Dictionary<string, JsonModule>());
        _store.Value ??= [];
        _jsonModules = _store.Value;
    }

    public IReadOnlyCollection<JsonModule> JsonModules => new ReadOnlyCollection<JsonModule>(_store.Value?.Values.ToList() ?? []);

    public JsonModule? GetJsonModule(string moduleId)
    {
        return _jsonModules.GetValueOrDefault(moduleId);
    }

    public JsonSchemaDataModel? GetJsonModuleDataModel(string moduleId)
    {
        JsonModule? jsonModule = GetJsonModule(moduleId);
        return jsonModule == null ? null : _dataModels.GetValueOrDefault(jsonModule);
    }

    public void AddJsonModule(JsonModule jsonModule)
    {
        LoadModule(jsonModule);
        _jsonModules.Add(jsonModule.ModuleId, jsonModule);
    }

    public void UpdateJsonModule(JsonModule jsonModule)
    {
        if (!_jsonModules.ContainsKey(jsonModule.ModuleId))
            throw new ArtemisPluginException("Cannot update a JSON module that has not been added yet");
        
        _plugin.GetFeature<JsonModulesWebApi>()?.DataModel.RemoveDynamicChildByKey(jsonModule.ModuleId);
        _dataModels.Remove(jsonModule);
        
        LoadModule(jsonModule);
        _jsonModules[jsonModule.ModuleId] = jsonModule;
    }

    public void SaveChanges()
    {
        _store.Save();
    }

    public void Load()
    {
        foreach (JsonModule jsonModule in _jsonModules.Values)
        {
            try
            {
                LoadModule(jsonModule);
            }
            catch (Exception e)
            {
                _logger.Warning(e, "Failed to load JSON module {JsonModule}", jsonModule.ModuleId);
            }
        }
    }

    public void Unload()
    {
        _dataModels.Clear();
    }

    private void LoadModule(JsonModule jsonModule)
    {
        string? title = jsonModule.Schema.GetTitle();
        if (title == null)
            throw new ArtemisPluginException("JSON module schema must have a title");
        string? description = jsonModule.Schema.GetDescription();

        JsonSchemaDataModel dataModel = new(jsonModule.Schema);
        _dataModels.Add(jsonModule, dataModel);
        _plugin.GetFeature<JsonModulesWebApi>()?.DataModel.AddDynamicChild(jsonModule.ModuleId, dataModel, title, description);
    }
}

public interface IJsonModuleService : IPluginService
{
    IReadOnlyCollection<JsonModule> JsonModules { get; }
    JsonModule? GetJsonModule(string moduleId);
    JsonSchemaDataModel? GetJsonModuleDataModel(string moduleId);
    void AddJsonModule(JsonModule jsonModule);
    void UpdateJsonModule(JsonModule jsonModule);
    void SaveChanges();
    void Load();
    void Unload();
}

public class JsonModule
{
    public string ModuleId { get; init; }
    public required JsonSchema Schema { get; set; }
}