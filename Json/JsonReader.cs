using System;
using System.Diagnostics.CodeAnalysis;

namespace Foster.Json
{
    /// <summary>
    /// Reads JSON from a Stream or Path
    /// </summary>
    public abstract class JsonReader
    {
        /// <summary>
        /// The current Token
        /// </summary>
        public JsonToken Token { get; protected set; }

        /// <summary>
        /// The current Value
        /// </summary>
        public object? Value { get; protected set; }

        /// <summary>
        /// The current Position in the Stream
        /// </summary>
        public abstract long Position { get; }

        /// <summary>
        /// Reads an Json Object from the Stream and returns it
        /// </summary>
        /// <param name="into">An optional object to read into. If null, it creates a new JsonObject</param>
        public JsonValue ReadObject()
        {
            var result = new JsonObject();
            var opened = false;

            while (Read() && Token != JsonToken.ObjectEnd)
            {
                if (!opened && Token == JsonToken.ObjectStart)
                {
                    opened = true;
                    continue;
                }

                if (Token != JsonToken.ObjectKey)
                    throw new Exception($"Expected Object Key");

                var key = Value as string;
                if (string.IsNullOrEmpty(key))
                    throw new Exception($"Invalid Object Key");

                result[key] = ReadValue();
            }

            return result;
        }

        /// <summary>
        /// Tries to read a JsonObject from the Stream
        /// </summary>
        public bool TryReadObject([MaybeNullWhen(false)] out JsonValue obj)
        {
            try 
            { 
                obj = ReadObject();
            }
            catch
            {
                // FIXME: this seems like the MaybeNullWhen attribute doesn't work?
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                obj = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Reads a JsonArray from the Stream
        /// </summary>
        public JsonValue ReadArray()
        {
            var arr = new JsonArray();
            while (Read() && Token != JsonToken.ArrayEnd)
                arr.Add(CurrentValue());
            return arr;
        }

        /// <summary>
        /// Reads a JsonValue from the Stream
        /// </summary>
        public JsonValue ReadValue()
        {
            Read();
            return CurrentValue();
        }

        private JsonValue CurrentValue()
        {
            switch (Token)
            {
                case JsonToken.Null:
                    return new JsonNull();

                case JsonToken.Boolean:
                    if (Value is bool Bool)
                        return Bool;
                    break;

                case JsonToken.Number:
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
                    if (Value is decimal Decimal)
                        return Decimal;
                    if (Value is float Float)
                        return Float;
                    if (Value is double Double)
                        return Double;
                    break;

                case JsonToken.String:
                    if (Value is string String)
                        return String;
                    break;

                case JsonToken.Binary:
                    if (Value is byte[] Bytes)
                        return Bytes;
                    break;

                case JsonToken.ObjectStart:
                    return ReadObject();

                case JsonToken.ArrayStart:
                    return ReadArray();

                case JsonToken.ObjectKey:
                case JsonToken.ObjectEnd:
                case JsonToken.ArrayEnd:
                    throw new Exception($"Unexpected {Token}");
            }

            return new JsonNull();
        }

        /// <summary>
        /// Skips the current Value
        /// </summary>
        public virtual void SkipValue()
        {
            ReadValue();
        }

        /// <summary>
        /// Reads the next Token in the Stream
        /// </summary>
        public abstract bool Read();
    }
}
