using Foster.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Json
{

    /// <summary>
    /// Encapsulates a Json Value
    /// </summary>
    public abstract class JsonValue
    {
        public readonly JsonType Type;

        public JsonValue(JsonType type)
        {
            Type = type;
        }

        public bool IsNull => Type == JsonType.Null;
        public bool IsBool => Type == JsonType.Bool;
        public bool IsNumber => Type == JsonType.Number;
        public bool IsString => Type == JsonType.String;
        public bool IsObject => Type == JsonType.Object;
        public bool IsArray => Type == JsonType.Array;
        public bool IsBinary => Type == JsonType.Binary;

        public abstract bool Bool { get; }
        public abstract byte Byte { get; }
        public abstract char Char { get; }
        public abstract short Short { get; }
        public abstract ushort UShort { get; }
        public abstract int Int { get; }
        public abstract uint UInt { get; }
        public abstract long Long { get; }
        public abstract ulong ULong { get; }
        public abstract decimal Decimal { get; }
        public abstract float Float { get; }
        public abstract double Double { get; }
        public abstract string String { get; }
        public abstract byte[] Bytes { get; }

        public T Enum<T>(T defaultValue = default) where T : struct, IConvertible
        {
            if (System.Enum.TryParse<T>(String, true, out var value))
                return value;
            return defaultValue;
        }

        public bool BoolOrDefault(bool defaultValue) => IsBool ? Bool : defaultValue;
        public byte ByteOrDefault(byte defaultValue) => IsNumber ? Byte : defaultValue;
        public char CharOrDefault(char defaultValue) => IsNumber ? Char : defaultValue;
        public short ShortOrDefault(short defaultValue) => IsNumber ? Short : defaultValue;
        public ushort UShortOrDefault(ushort defaultValue) => IsNumber ? UShort : defaultValue;
        public int IntOrDefault(int defaultValue) => IsNumber ? Int : defaultValue;
        public uint UIntOrDefault(uint defaultValue) => IsNumber ? UInt : defaultValue;
        public long LongOrDefault(long defaultValue) => IsNumber ? Long : defaultValue;
        public ulong ULongOrDefault(ulong defaultValue) => IsNumber ? ULong : defaultValue;
        public decimal DecimalOrDefault(decimal defaultValue) => IsNumber ? Decimal : defaultValue;
        public float FloatOrDefault(float defaultValue) => IsNumber ? Float : defaultValue;
        public double DoubleOrDefault(double defaultValue) => IsNumber ? Double : defaultValue;
        public string StringOrDefault(string defaultValue) => IsString ? String : defaultValue;

        public abstract JsonValue this[string key] { get; set; }
        public abstract JsonValue this[int index] { get; set; }

        public abstract IEnumerable<string> Keys { get; }
        public abstract IEnumerable<JsonValue> Values { get; }
        public abstract IEnumerable<JsonValue> Array { get; }
        public abstract IEnumerable<KeyValuePair<string, JsonValue>> Object { get; }
        public abstract int Count { get; }

        public abstract object? UnderlyingValue { get; }
        public abstract int GetHashedValue();

        public abstract JsonValue Clone();

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
        public static implicit operator JsonValue(string value) => new JsonValue<string>(JsonType.String, value);
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
        public override IEnumerable<JsonValue> Array => Enumerable.Empty<JsonValue>();
        public override IEnumerable<KeyValuePair<string, JsonValue>> Object => Enumerable.Empty<KeyValuePair<string, JsonValue>>();
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

        public override int Count => 0;
        public override IEnumerable<string> Keys => Enumerable.Empty<string>();
        public override IEnumerable<JsonValue> Values => Enumerable.Empty<JsonValue>();
        public override IEnumerable<JsonValue> Array => Enumerable.Empty<JsonValue>();
        public override IEnumerable<KeyValuePair<string, JsonValue>> Object => Enumerable.Empty<KeyValuePair<string, JsonValue>>();

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
