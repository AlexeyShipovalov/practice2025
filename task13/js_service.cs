using System;
using task13;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
public static class JsonService
{
    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new DateTimeConverter("yyyy-MM-dd") },
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
    public static string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, GetJsonOptions());
    }
    public static Student Deserialize(string json)
    {
         try
        {
            var result = JsonSerializer.Deserialize<Student>(json, GetJsonOptions());
            return result ?? throw new InvalidOperationException("десериализация вернула 0");
        }
        catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
        {
            throw new InvalidOperationException("Неверный JSON формат", ex);
        }
    }
    public static void SaveToFile(Student student, string filePath)
    {
        File.WriteAllText(filePath, Serialize(student));
    }
    public static Student LoadFromFile(string filePath)
    {
        return Deserialize(File.ReadAllText(filePath));
    }
}
public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;
    public DateTimeConverter(string format) => _format = format;
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => 
        DateTime.ParseExact(reader.GetString(), _format, null);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => 
        writer.WriteStringValue(value.ToString(_format));
}
