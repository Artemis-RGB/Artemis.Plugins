﻿using Artemis.Core;
using Artemis.Plugins.Nodes.General.Nodes.DataModel.Screens;
using Artemis.Storage.Entities.Profile;

namespace Artemis.Plugins.Nodes.General.Nodes.DataModel;

[Node("Data Model-Event", "Outputs the latest values of a data model event.", "Data Model", OutputType = typeof(object))]
public class DataModelEventNode : Node<DataModelPathEntity, DataModelEventNodeCustomViewModel>, IDisposable
{
    private readonly ObjectOutputPins _objectOutputPins;
    private IDataModelEvent? _dataModelEvent;
    private DataModelPath? _dataModelPath;
    private DateTime _lastTrigger;
    private object? _lastValue;
    private OutputPin? _newValuePin;
    private OutputPin? _oldValuePin;
    private int _valueChangeCount;

    public DataModelEventNode()
    {
        _objectOutputPins = new ObjectOutputPins(this);

        TimeSinceLastTrigger = CreateOutputPin<Numeric>("Time since trigger");
        TriggerCount = CreateOutputPin<Numeric>("Trigger count");

        // Monitor storage for changes
        StorageModified += (_, _) => UpdateDataModelPath();
    }

    public INodeScript? Script { get; set; }
    public OutputPin<Numeric> TimeSinceLastTrigger { get; }
    public OutputPin<Numeric> TriggerCount { get; }

    public override void Initialize(INodeScript script)
    {
        Script = script;

        if (Storage == null)
            return;

        UpdateDataModelPath();
    }

    public override void Evaluate()
    {
        object? pathValue = _dataModelPath?.GetValue();

        // If the path is a data model event, evaluate the event
        if (pathValue is IDataModelEvent dataModelEvent)
        {
            TimeSinceLastTrigger.Value = dataModelEvent.TimeSinceLastTrigger.TotalMilliseconds;
            TriggerCount.Value = dataModelEvent.TriggerCount;

            _objectOutputPins.SetCurrentValue(dataModelEvent.LastEventArgumentsUntyped);
        }
        // If the path is a regular value, evaluate the current value
        else if (_oldValuePin != null && _newValuePin != null)
        {
            if (_newValuePin.IsNumeric)
                pathValue = new Numeric(pathValue);

            if (Equals(_lastValue, pathValue))
            {
                TimeSinceLastTrigger.Value = (DateTime.Now - _lastTrigger).TotalMilliseconds;
                return;
            }

            _valueChangeCount++;
            _lastTrigger = DateTime.Now;

            _oldValuePin.Value = _lastValue;
            _newValuePin.Value = pathValue;

            _lastValue = pathValue;

            TimeSinceLastTrigger.Value = 0;
            TriggerCount.Value = _valueChangeCount;
        }
    }

    public override IEnumerable<PluginFeature> GetFeatureDependencies()
    {
        return base.GetFeatureDependencies().Concat(_dataModelPath?.GetFeatureDependencies() ?? []);
    }

    private void UpdateDataModelPath()
    {
        DataModelPath? old = _dataModelPath;
        _dataModelPath = Storage != null ? new DataModelPath(Storage) : null;
        if (_dataModelPath != null)
            _dataModelPath.PathValidated += DataModelPathOnPathValidated;

        if (old != null)
        {
            old.PathValidated -= DataModelPathOnPathValidated;
            old.Dispose();
        }

        UpdateOutputPins();
    }

    private void UpdateOutputPins()
    {
        object? pathValue = _dataModelPath?.GetValue();

        if (pathValue is IDataModelEvent dataModelEvent)
            CreateEventPins(dataModelEvent);
        else
            CreateValuePins();
    }

    private void CreateEventPins(IDataModelEvent dataModelEvent)
    {
        if (_dataModelEvent == dataModelEvent)
            return;

        ClearPins();
        _dataModelEvent = dataModelEvent;
        _objectOutputPins.ChangeType(dataModelEvent.ArgumentsType);
    }

    private void CreateValuePins()
    {
        ClearPins();

        Type? propertyType = _dataModelPath?.GetPropertyType();
        if (propertyType == null)
            return;

        if (Numeric.IsTypeCompatible(propertyType))
            propertyType = typeof(Numeric);

        _oldValuePin = CreateOrAddOutputPin(propertyType, "Old value");
        _newValuePin = CreateOrAddOutputPin(propertyType, "New value");
        _lastValue = null;
        _valueChangeCount = 0;
    }

    private void ClearPins()
    {
        // Clear the output pins by changing the type to null
        _objectOutputPins.ChangeType(null);

        if (_oldValuePin != null)
        {
            RemovePin(_oldValuePin);
            _oldValuePin = null;
        }

        if (_newValuePin != null)
        {
            RemovePin(_newValuePin);
            _newValuePin = null;
        }
    }

    private void DataModelPathOnPathValidated(object? sender, EventArgs e)
    {
        // Update the output pin now that the type is known and attempt to restore the connection that was likely missing
        UpdateOutputPins();
        Script?.LoadConnections();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _dataModelPath?.Dispose();
    }
}