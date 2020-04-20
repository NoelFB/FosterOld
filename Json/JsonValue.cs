using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Foster.Json
{

    /// <summary>
    /// Encapsulates a Json Value
    /// </summary>
    public abstract class JsonValue
    {
        /// <summary>
        /// The Type of Json Value
        /// </summary>
        public readonly JsonType Type;

        /// <summary>
        /// Creates a new Json Value of the given type
        /// </summary>
        public JsonValue(JsonType type)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a Json Value from a File
        /// </summary>
        public static JsonValue FromFile(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new JsonTextReader(stream);
            return reader.ReadObject();
        }

        /// <summary>
        /// Creates a Json Value from a String
        /// </summary>
        public static JsonValue FromString(string jsonString)
        {
            using var reader = new JsonTextReader(new StringReader(jsonString));
            return reader.ReadObject();
        }

        /// <summary>
        /// Returns true if the Json value is Null
        /// </summary>
        public bool IsNull => Type == JsonType.Null;

        /// <summary>
        /// Returns true if the Json Value is a Bool
        /// </summary>
        public bool IsBool => Type == JsonType.Bool;

        /// <summary>
        /// Returns true if the Json Value is a Number
        /// </summary>
        public bool IsNumber => Type == JsonType.Number;

        /// <summary>
        /// Returns true if the Json Value is a String
        /// </summary>
        public bool IsString => Type == JsonType.String;

        /// <summary>
        /// Returns true if the Json Value is an Object
        /// </summary>
        public bool IsObject => Type == JsonType.Object;

        /// <summary>
        /// Returns true if the Json Value is an Array
        /// </summary>
        public bool IsArray => Type == JsonType.Array;

        /// <summary>
        /// Returns true if the Json Value is a Binary Data
        /// </summary>
        public bool IsBinary => Type == JsonType.Binary;

        /// <summary>
        /// Returns the bool value of the Json Value
        /// </summary>
        public abstract bool Bool { get; }

        /// <summary>
        /// Returns the byte value of the Json Value
        /// </summary>
        public abstract byte Byte { get; }

        /// <summary>
        /// Returns the char value of the Json Value
        /// </summary>
        public abstract char Char { get; }

        /// <summary>
        /// Returns the short value of the Json Value
        /// </summary>
        public abstract short Short { get; }

        /// <summary>
        /// Returns the ushort value of the Json Value
        /// </summary>
        public abstract ushort UShort { get; }

        /// <summary>
        /// Returns the int value of the Json Value
        /// </summary>
        public abstract int Int { get; }

        /// <summary>
        /// Returns the uint value of the Json Value
        /// </summary>
        public abstract uint UInt { get; }

        /// <summary>
        /// Returns the long value of the Json Value
        /// </summary>
        public abstract long Long { get; }

        /// <summary>
        /// Returns the ulong value of the Json Value
        /// </summary>
        public abstract ulong ULong { get; }

        /// <summary>
        /// Returns the decimal value of the Json Value
        /// </summary>
        public abstract decimal Decimal { get; }

        /// <summary>
        /// Returns the float value of the Json Value
        /// </summary>
        public abstract float Float { get; }

        /// <summary>
        /// Returns the double value of the Json Value
        /// </summary>
        public abstract double Double { get; }

        /// <summary>
        /// Returns the string value of the Json Value
        /// </summary>
        public abstract string String { get; }

        /// <summary>
        /// Returns the bytes value of the Json Value
        /// </summary>
        public abstract byte[] Bytes { get; }

        /// <summary>
        /// Returns the Enum value, or a default value if it isn't one
        /// </summary>
        public T Enum<T>(T defaultValue = default) where T : struct, IConvertible
        {
            if (System.Enum.TryParse<T>(String, true, out var value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Returns the bool value, or a default value if it isn't one
        /// </summary>
        public bool BoolOrDefault(bool defaultValue) => IsBool ? Bool : defaultValue;

        /// <summary>
        /// Returns the byte value, or a default value if it isn't one
        /// </summary>
        public byte ByteOrDefault(byte defaultValue) => IsNumber ? Byte : defaultValue;

        /// <summary>
        /// Returns the char value, or a default value if it isn't one
        /// </summary>
        public char CharOrDefault(char defaultValue) => IsNumber ? Char : defaultValue;

        /// <summary>
        /// Returns the short value, or a default value if it isn't one
        /// </summary>
        public short ShortOrDefault(short defaultValue) => IsNumber ? Short : defaultValue;

        /// <summary>
        /// Returns the ushort value, or a default value if it isn't one
        /// </summary>
        public ushort UShortOrDefault(ushort defaultValue) => IsNumber ? UShort : defaultValue;

        /// <summary>
        /// Returns the int value, or a default value if it isn't one
        /// </summary>
        public int IntOrDefault(int defaultValue) => IsNumber ? Int : defaultValue;

        /// <summary>
        /// Returns the uint value, or a default value if it isn't one
        /// </summary>
        public uint UIntOrDefault(uint defaultValue) => IsNumber ? UInt : defaultValue;

        /// <summary>
        /// Returns the long value, or a default value if it isn't one
        /// </summary>
        public long LongOrDefault(long defaultValue) => IsNumber ? Long : defaultValue;

        /// <summary>
        /// Returns the ulong value, or a default value if it isn't one
        /// </summary>
        public ulong ULongOrDefault(ulong defaultValue) => IsNumber ? ULong : defaultValue;

        /// <summary>
        /// Returns the decimal value, or a default value if it isn't one
        /// </summary>
        public decimal DecimalOrDefault(decimal defaultValue) => IsNumber ? Decimal : defaultValue;

        /// <summary>
        /// Returns the float value, or a default value if it isn't one
        /// </summary>
        public float FloatOrDefault(float defaultValue) => IsNumber ? Float : defaultValue;

        /// <summary>
        /// Returns the double value, or a default value if it isn't one
        /// </summary>
        public double DoubleOrDefault(double defaultValue) => IsNumber ? Double : defaultValue;

        /// <summary>
        /// Returns the string value, or a default value if it isn't one
        /// </summary>
        public string StringOrDefault(string defaultValue) => IsString ? String : defaultValue;

        /// <summary>
        /// Gets the Json Value of the given Key, if this is an Object
        /// Otherwise returns a JsonNull value
        /// </summary>
        public abstract JsonValue this[string key] { get; set; }

        /// <summary>
        /// Gets the Json Value of the given Index, if this is an Array.
        /// Otherwise returns a JsonNull value.
        /// </summary>
        public abstract JsonValue this[int index] { get; set; }

        /// <summary>
        /// Adds a value if this is an Array.
        /// Otherwise throws an InvalidOprationException
        /// </summary>
        public abstract void Add(JsonValue value);

        /// <summary>
        /// Removes a value if this is an Array.
        /// Otherwise throws an InvalidOprationException
        /// </summary>
        public abstract void Remove(JsonValue value);

        /// <summary>
        /// Returns an Enumerable of all the Keys, or an empty list of this is not an Object
        /// </summary>
        public abstract IEnumerable<string> Keys { get; }

        /// <summary>
        /// Returns an Enumerable of all the Values, or an empty list of this is not an Object or Array
        /// </summary>
        public abstract IEnumerable<JsonValue> Values { get; }

        /// <summary>
        /// Returns an Enumerable of all the Keys-Value pairs, or an empty list of this is not an Object
        /// </summary>
        public abstract IEnumerable<KeyValuePair<string, JsonValue>> Pairs { get; }

        /// <summary>
        /// Returns the total amount of Array Entries or Key-Value Pairs
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// The underlying C# Object
        /// </summary>
        public abstract object? UnderlyingValue { get; }

        /// <summary>
        /// Gets a unique hashed value based on the contents of the Json Data
        /// </summary>
        public abstract int GetHashedValue();

        /// <summary>
        /// Clones the Json Value
        /// </summary>
        /// <returns></returns>
        public abstract JsonValue Clone();

        /// <summary>
        /// Creates a new file with the contents of this Json Value
        /// </summary>
        public void ToFile(string path, bool strict = true)
        {
            using var writer = new JsonTextWriter(File.Create(path), strict);
            writer.Json(this);
        }

        public static implicit operator JsonValue(bool value) => new JsonValue<bool>(JsonType.Bool, value);
        public static implicit operator JsonValue(decimal value) => new JsonValue<decimal>(JsonType.Number, value);
        public static implicit operator JsonValue(float value) => new JsonValue<float>(JsonType.Number, value);
        public static implicit operator JsonValue(double value) => new JsonValue<double>(JsonType.Number, value);
        public static implicit operator JsonValue(byte value) => new JsonValue<byte>(JsonType.Number, value);
        public static implicit operator JsonValue(char value) => new JsonValue<char>(JsonType.Number, value);
        public static implicit operator JsonValue(short value) => new JsonValue<short>(JsonType.Number, value);
        public static implicit operator JsonValue(ushort value) => new JsonValue<ushort>(JsonType.Number, value);
        public static implicit operator JsonValue(int value) => new JsonValue<int>(JsonType.Number, value);
        public static implicit operator JsonValue(uint value) => new JsonValue<uint>(JsonType.Number, value);
        public static implicit operator JsonValue(long value) => new JsonValue<long>(JsonType.Number, value);
        public static implicit operator JsonValue(ulong value) => new JsonValue<ulong>(JsonType.Number, value);
        public static implicit operator JsonValue(string? value) => new JsonValue<string>(JsonType.String, value ?? "");
        public static implicit operator JsonValue(List<string> value) => new JsonArray(value);
        public static implicit operator JsonValue(string[] value) => new JsonArray(value);
        public static implicit operator JsonValue(byte[] value) => new JsonValue<byte[]>(JsonType.Binary, value);

        public static implicit operator bool(JsonValue value) => value.Bool;
        public static implicit operator float(JsonValue value) => value.Float;
        public static implicit operator double(JsonValue value) => value.Double;
        public static implicit operator byte(JsonValue value) => value.Byte;
        public static implicit operator char(JsonValue value) => value.Char;
        public static implicit operator short(JsonValue value) => value.Short;
        public static implicit operator ushort(JsonValue value) => value.UShort;
        public static implicit operator int(JsonValue value) => value.Int;
        public static implicit operator uint(JsonValue value) => value.UInt;
        public static implicit operator long(JsonValue value) => value.Long;
        public static implicit operator ulong(JsonValue value) => value.ULong;
        public static implicit operator string(JsonValue value) => value.String;
        public static implicit operator byte[](JsonValue value) => value.Bytes;

    }

    /// <summary>
    /// A Null Json Value
    /// </summary>
    public class JsonNull : JsonValue
    {
        internal static readonly JsonNull nul = new JsonNull();
        internal static readonly byte[] binary = new byte[0];

        public JsonNull() : base(JsonType.Null)
        {

        }

        public override bool Bool => false;
        public override decimal Decimal => 0;
        public override float Float => 0;
        public override double Double => 0;
        public override byte Byte => 0;
        public override char Char => (char)0;
        public override short Short => 0;
        public override ushort UShort => 0;
        public override int Int => 0;
        public override uint UInt => 0;
        public override long Long => 0;
        public override ulong ULong => 0;
        public override string String => string.Empty;
        public override byte[] Bytes => binary;
        public override object? UnderlyingValue => null;
        public override int GetHashedValue() => 0;

        public override void Add(JsonValue value)
        {
            throw new InvalidOperationException();
        }

        public override void Remove(JsonValue value)
        {
            throw new InvalidOperationException();
        }

        public override JsonValue this[string key]
        {
            get => nul;
            set => throw new InvalidOperationException();
        }

        public override JsonValue this[int index]
        {
            get => nul;
            set => throw new InvalidOperationException();
        }

        public override JsonValue Clone()
        {
            return nul;
        }

        public override int Count => 0;
        public override IEnumerable<string> Keys => Enumerable.Empty<string>();
        public override IEnumerable<JsonValue> Values => Enumerable.Empty<JsonValue>();
        public override IEnumerable<KeyValuePair<string, JsonValue>> Pairs => Enumerable.Empty<KeyValuePair<string, JsonValue>>();
    }

    /// <summary>
    /// A Json Value with a given C# data type
    /// </summary>
    public class JsonValue<T> : JsonValue
    {

        public readonly T Value;

        public JsonValue(JsonType type, T value) : base(type)
        {
            Value = value;
        }

        public override bool Bool => (Value is bool value ? value : false);

        public override decimal Decimal
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is decimal value)
                        return value;
                    return Convert.ToDecimal(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && decimal.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override float Float
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is float value)
                        return value;
                    return Convert.ToSingle(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && float.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override double Double
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is double value)
                        return value;
                    return Convert.ToDouble(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && double.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override short Short
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is short value)
                        return value;
                    return Convert.ToInt16(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && short.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override byte Byte
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is byte value)
                        return value;
                    return Convert.ToByte(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && byte.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override char Char
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is char value)
                        return value;
                    return Convert.ToChar(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && char.TryParse(value, out var n))
                    return n;

                return (char)0;
            }
        }

        public override ushort UShort
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is ushort value)
                        return value;
                    return Convert.ToUInt16(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && ushort.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override int Int
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is int value)
                        return value;
                    return Convert.ToInt32(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && int.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override uint UInt
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is uint value)
                        return value;
                    return Convert.ToUInt32(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && uint.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override long Long
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is long value)
                        return value;
                    return Convert.ToInt64(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && long.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override ulong ULong
        {
            get
            {
                if (IsNumber)
                {
                    if (Value is ulong value)
                        return value;
                    return Convert.ToUInt64(Value, NumberFormatInfo.InvariantInfo);
                }
                else if (IsString && Value is string value && ulong.TryParse(value, out var n))
                    return n;

                return 0;
            }
        }

        public override string String
        {
            get
            {
                if (IsString && Value is string str)
                    return str;
                else if (Value != null)
                    return Value.ToString() ?? "";
                return "";
            }
        }

        public override byte[] Bytes
        {
            get
            {
                if (IsBinary && Value is byte[] bytes)
                    return bytes;
                return JsonNull.binary;
            }
        }

        public override void Add(JsonValue value)
        {
            throw new InvalidOperationException();
        }

        public override void Remove(JsonValue value)
        {
            throw new InvalidOperationException();
        }

        public override int Count => 0;
        public override IEnumerable<string> Keys => Enumerable.Empty<string>();
        public override IEnumerable<JsonValue> Values => Enumerable.Empty<JsonValue>();
        public override IEnumerable<KeyValuePair<string, JsonValue>> Pairs => Enumerable.Empty<KeyValuePair<string, JsonValue>>();

        public override object? UnderlyingValue => Value;

        public override int GetHashedValue()
        {
            if (IsString)
                return Calc.StaticStringHash(String);

            if (IsNumber)
                return Int;

            if (IsBool)
                return (Bool ? 1 : 0);

            if (IsBinary)
                return (int)Calc.Adler32(0, Bytes);

            return 0;
        }

        public override JsonValue Clone()
        {
            if (IsString)
                return String;

            if (IsNumber)
            {
                if (Value is float Float)
                    return Float;
                if (Value is double Double)
                    return Double;
                if (Value is byte Byte)
                    return Byte;
                if (Value is char Char)
                    return Char;
                if (Value is short Short)
                    return Short;
                if (Value is ushort UShort)
                    return UShort;
                if (Value is int Int)
                    return Int;
                if (Value is uint UInt)
                    return UInt;
                if (Value is long Long)
                    return Long;
                if (Value is ulong ULong)
                    return ULong;
            }

            if (IsBool)
                return Bool;

            if (IsBinary)
            {
                var bytes = new byte[Bytes.Length];
                System.Array.Copy(Bytes, 0, bytes, 0, bytes.Length);
                return bytes;
            }

            return JsonNull.nul;
        }

        public override JsonValue this[string key]
        {
            get => JsonNull.nul;
            set => throw new InvalidOperationException();
        }

        public override JsonValue this[int index]
        {
            get => JsonNull.nul;
            set => throw new InvalidOperationException();
        }
    }
}
