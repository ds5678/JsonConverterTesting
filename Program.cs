using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConverterTesting;

internal class Program
{
    static void Main()
    {
        Console.WriteLine(JsonSerializer.Serialize(new RecordWithHex32(1, 0x12345678), RecordSerializerContext.Default.RecordWithHex32));
        Console.WriteLine(JsonSerializer.Serialize(new RecordWithInt32Pairs(Int32Pair1.FromInt64(0x1234567890ABCDEF), Int32Pair2.FromInt64(0x1234567890ABCDEF)), RecordSerializerContext.Default.RecordWithInt32Pairs));
        Console.WriteLine("Done!");
    }
}

public sealed class Hex32Converter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();
        return Convert.ToInt32(hex, 16);
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("X8"));
    }
}

public sealed class Int32Pair1Converter : JsonConverter<Int32Pair1>
{
    public override Int32Pair1 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Int32Pair1.FromInt64(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, Int32Pair1 value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToInt64());
    }
}

public sealed class Int32Pair2Converter : JsonConverter<Int32Pair2>
{
    public override Int32Pair2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Int32Pair2.FromInt64(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, Int32Pair2 value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToInt64());
    }
}

public readonly record struct Int32Pair1(int High, int Low)
{
    public long ToInt64() => (long)High << 32 | (uint)Low;
    public static Int32Pair1 FromInt64(long value) => new Int32Pair1((int)(value >> 32), (int)value);
}

[JsonConverter(typeof(Int32Pair2Converter))]
public readonly record struct Int32Pair2(int High, int Low)
{
    public long ToInt64() => (long)High << 32 | (uint)Low;
    public static Int32Pair2 FromInt64(long value) => new Int32Pair2((int)(value >> 32), (int)value);
}

public readonly record struct RecordWithHex32(int Normal, [property: JsonConverter(typeof(Hex32Converter))] int Hex32);

public readonly record struct RecordWithInt32Pairs([property: JsonConverter(typeof(Int32Pair1Converter))] Int32Pair1 Pair1, Int32Pair2 Pair2);

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
[JsonSerializable(typeof(RecordWithHex32))]
[JsonSerializable(typeof(RecordWithInt32Pairs))]
public sealed partial class RecordSerializerContext : JsonSerializerContext
{
}