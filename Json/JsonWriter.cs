using System;

namespace Foster.Json
{
    /// <summary>
    /// Writes Json to a string format
    /// </summary>
    public abstract class JsonWriter
    {
        public abstract void Key(string name);
        public abstract void ObjectBegin();
        public abstract void ObjectEnd();
        public abstract void ArrayBegin();
        public abstract void ArrayEnd();
        public abstract void Comment(string text);

        public abstract void Null();
        public abstract void Value(bool value);
        public abstract void Value(byte value);
        public abstract void Value(char value);
        public abstract void Value(short value);
        public abstract void Value(ushort value);
        public abstract void Value(int value);
        public abstract void Value(uint value);
        public abstract void Value(long value);
        public abstract void Value(ulong value);
        public abstract void Value(decimal value);
        public abstract void Value(float value);
        public abstract void Value(double value);
        public abstract void Value(string value);


        public void Value(byte[] value) => Value(value.AsSpan());
        public abstract void Value(ReadOnlySpan<byte> value);

        public void Json(JsonValue value)
        {
            if (value != null)
            {
                switch (value.Type)
                {
                    case JsonType.Object:
                        if (value.IsObject)
                        {
                            ObjectBegin();
                            foreach (var pair in value.Pairs)
                            {
                                Key(pair.Key);
                                Json(pair.Value);
                            }
                            ObjectEnd();
                            return;
                        }
                        break;

                    case JsonType.Array:
                        if (value.IsArray)
                        {
                            ArrayBegin();
                            foreach (var item in value.Values)
                                Json(item);
                            ArrayEnd();
                            return;
                        }
                        break;

                    case JsonType.Bool:
                        Value(value.Bool);
                        return;

                    case JsonType.String:
                        Value(value.String);
                        return;

                    case JsonType.Number:
                        {
                            if (value is JsonValue<bool> Bool)
                            {
                                Value(Bool.Bool);
                                return;
                            }
                            else if (value is JsonValue<decimal> Decimal)
                            {
                                Value(Decimal.Decimal);
                                return;
                            }
                            else if (value is JsonValue<float> Float)
                            {
                                Value(Float.Float);
                                return;
                            }
                            else if (value is JsonValue<double> Double)
                            {
                                Value(Double.Double);
                                return;
                            }
                            else if (value is JsonValue<byte> Byte)
                            {
                                Value(Byte.Byte);
                                return;
                            }
                            else if (value is JsonValue<char> Char)
                            {
                                Value(Char.Char);
                                return;
                            }
                            else if (value is JsonValue<short> Short)
                            {
                                Value(Short.Short);
                                return;
                            }
                            else if (value is JsonValue<ushort> UShort)
                            {
                                Value(UShort.UShort);
                                return;
                            }
                            else if (value is JsonValue<int> Int)
                            {
                                Value(Int.Int);
                                return;
                            }
                            else if (value is JsonValue<uint> UInt)
                            {
                                Value(UInt.UInt);
                                return;
                            }
                            else if (value is JsonValue<long> Long)
                            {
                                Value(Long.Long);
                                return;
                            }
                            else if (value is JsonValue<ulong> ULong)
                            {
                                Value(ULong.ULong);
                                return;
                            }
                        }
                        break;
                    case JsonType.Binary:
                        if (value is JsonValue<byte[]> Bytes)
                        {
                            Value(Bytes.Bytes);
                            return;
                        }
                        break;
                }
            }

            Null();
        }
    }
}
