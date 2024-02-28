using System.Globalization;
using System.Text.Json;
using Artemis.Core;
using Avalonia.Data.Converters;

namespace Artemis.Plugins.Nodes.General.Converters;

/// <summary>
///     Converts input into <see cref="Numeric" />.
/// </summary>
public class JsonConverter : IValueConverter
{
    private readonly JsonSerializerOptions _options;

    public JsonConverter()
    {
        _options = new JsonSerializerOptions(CoreJson.GetJsonSerializerOptions());
        _options.WriteIndented = true;
        
    }
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return JsonSerializer.Serialize(value, _options);
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? json = value?.ToString();
        return json == null ? null : JsonSerializer.Deserialize(json, targetType);
    }
}