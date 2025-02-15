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
    private readonly PluginSetting<Dictionary<Guid, JsonModule>> _store;
    private readonly Dictionary<Guid, JsonModule> _jsonModules;
    private readonly Dictionary<JsonModule, JsonSchemaDataModel> _dataModels = [];
    private readonly JsonModulesWebApi? _module;

    public JsonModuleService(ILogger logger, PluginSettings pluginSettings, Plugin plugin)
    {
        _logger = logger;
        _store = pluginSettings.GetSetting("JsonModules", new Dictionary<Guid, JsonModule>());
        _store.Value ??= [];
        _jsonModules = _store.Value;
        _module = plugin.GetFeature<JsonModulesWebApi>();

        Load();
    }

    public IReadOnlyCollection<JsonModule> JsonModules => new ReadOnlyCollection<JsonModule>(_store.Value?.Values.ToList() ?? []);

    public JsonModule? GetJsonModule(Guid moduleId)
    {
        return _jsonModules.GetValueOrDefault(moduleId);
    }

    public JsonSchemaDataModel? GetJsonModuleDataModel(Guid moduleId)
    {
        JsonModule? jsonModule = GetJsonModule(moduleId);
        return jsonModule == null ? null : _dataModels.GetValueOrDefault(jsonModule);
    }

    public void AddJsonModule(JsonModule jsonModule)
    {
        _jsonModules.Add(jsonModule.ModuleId, jsonModule);
        LoadModule(jsonModule);
    }

    public void SaveChanges()
    {
        _store.Save();
    }

    private void Load()
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

    private void LoadModule(JsonModule jsonModule)
    {
        string? title = jsonModule.Schema.GetTitle();
        if (title == null)
            throw new ArtemisPluginException("JSON module schema must have a title");
        string? description = jsonModule.Schema.GetDescription();

        JsonSchemaDataModel dataModel = new(jsonModule.Schema);
        _dataModels.Add(jsonModule, dataModel);
        _module?.DataModel.AddDynamicChild(jsonModule.ModuleId.ToString(), dataModel, title, description);
    }
}

public interface IJsonModuleService : IPluginService
{
    IReadOnlyCollection<JsonModule> JsonModules { get; }
    JsonModule? GetJsonModule(Guid moduleId);
    JsonSchemaDataModel? GetJsonModuleDataModel(Guid moduleId);
    void AddJsonModule(JsonModule jsonModule);
    void SaveChanges();
}

public class JsonModule
{
    public Guid ModuleId { get; init; }
    public required JsonSchema Schema { get; set; }
}