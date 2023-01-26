using System.Text.Json;
using LAPS.Lib.Models.Json;

namespace LAPS.Lib;

/// <summary>
/// Base class used for classes that can be converted to and from a JSON document.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JsonOutItem<T>
{
    /// <summary>
    /// Convert the item to a JSON document.
    /// </summary>
    /// <returns>A string representation of the item as a JSON document.</returns>
    public string ToJson()
    {
        JsonSerializerOptions serializerOptions = new()
        {
            Converters = { new JsonDateTimeOffsetConverter() }
        };

        return JsonSerializer.Serialize(this, typeof(T), serializerOptions);
    }

    /// <summary>
    /// Create an instance of <see cref="T"/> from a JSON document.
    /// </summary>
    /// <param name="jsonString">The string representation of the JSON document.</param>
    /// <returns>An instance of the JSON document as <see cref="T"/></returns>
    public static T FromJson(string jsonString)
    {
        JsonSerializerOptions serializerOptions = new()
        {
            Converters = { new JsonDateTimeOffsetConverter() }
        };

        return JsonSerializer.Deserialize<T>(jsonString, serializerOptions);
    }
}