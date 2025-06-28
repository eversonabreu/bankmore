using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BankMore.Core.Utils.Extensions;

public static class StringExtensions
{
    public static string OnlyNumbers(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var chr = input.Where(char.IsDigit);
        return new string([.. chr]);
    }

    public static string ComputeMd5Hash(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    public static string GetDeterministicJson(this string json)
    {
        using var document = JsonDocument.Parse(json);
        return SerializeCanonical(document.RootElement);
    }

    private static string SerializeCanonical(JsonElement element)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            WriteCanonicalElement(element, writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteCanonicalElement(JsonElement element, Utf8JsonWriter writer)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject().OrderBy(p => p.Name))
                {
                    writer.WritePropertyName(property.Name);
                    WriteCanonicalElement(property.Value, writer);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                    WriteCanonicalElement(item, writer);
                writer.WriteEndArray();
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }
}