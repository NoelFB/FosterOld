using System;
using System.IO;

namespace Foster.Framework.Json
{
    /// <summary>
    /// Writes Json to a string format
    /// </summary>
    public class JsonWriter : IDisposable
    {

        /// <summary>
        /// The New Line character to use
        /// </summary>
        public string NewlineString = "\n";

        /// <summary>
        /// The Tab character to use for indents
        /// </summary>
        public string TabString = "\t";

        /// <summary>
        /// Whether the output should be Verbose
        /// </summary>
        public bool Verbose = true;

        /// <summary>
        /// Whether the output should be Strict (true) Json or not
        /// When false, the output will look more like hjson
        /// </summary>
        public bool Strict = true;

        private readonly TextWriter writer;
        private int depth = 0;
        private bool wasValue;
        private bool wasBracket;

        public JsonWriter(string path, bool strict = true) : this(File.Open(path, FileMode.Create), strict)
        {
        }

        public JsonWriter(Stream stream, bool strict = true) : this(new StreamWriter(stream), strict)
        {
        }

        public JsonWriter(TextWriter writer, bool strict = true)
        {
            this.writer = writer;
            Strict = strict;
        }

        private void Next(bool isValue = false, bool isKey = false, bool isBracket = false)
        {
            if (wasValue && (Strict || !Verbose))
                writer.Write(",");
            if ((wasValue || (wasBracket && !isBracket) || isKey) && Verbose)
                Newline();
            if (wasBracket && isBracket && Verbose)
                writer.Write(" ");

            wasValue = isValue;
            wasBracket = isBracket;
        }

        private void Newline()
        {
            writer.Write(NewlineString);
            for (int i = 0; i < depth; i++)
                writer.Write(TabString);
        }

        private void ContainerBegin(char id)
        {
            Next(isBracket: true);
            writer.Write(id);
            depth++;
        }

        private void ContainerEnd(char id)
        {
            depth--;

            if (Verbose)
            {
                if (wasBracket)
                    writer.Write(" ");
                else
                    Newline();
            }

            writer.Write(id);

            wasValue = true;
            wasBracket = false;
        }

        public void Key(string name)
        {
            Next(isKey: true);
            EscapedString(name);

            writer.Write(":");
            if (Verbose)
                writer.Write(" ");
        }

        public void ObjectBegin()
        {
            ContainerBegin('{');
        }

        public void ObjectEnd()
        {
            ContainerEnd('}');
        }

        public void ArrayBegin()
        {
            ContainerBegin('[');
        }

        public void ArrayEnd()
        {
            ContainerEnd(']');
        }

        public void Comment(string text)
        {
            if (!Strict && Verbose && text.Length > 0)
            {
                ReadOnlySpan<char> span = text;

                int last = 0, next = 0;
                while ((next = text.IndexOf('\n', last)) >= 0)
                {
                    writer.Write("# ");
                    writer.Write(span.Slice(last, next - last));
                    Newline();
                    last = next + 1;
                }

                writer.Write("# ");
                writer.Write(span.Slice(last));
            }
        }

        public void Null()
        {
            Next(isValue: true);
            writer.Write("null");
        }

        public void Value(bool value)
        {
            Next(isValue: true);
            writer.Write(value ? "true" : "false");
        }

        public void Value(char value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(short value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(int value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(long value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(float value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(double value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public void Value(string value)
        {
            Next(isValue: true);
            EscapedString(value);
        }

        public void Json(JsonValue value)
        {
            if (value != null)
            {
                switch (value.Type)
                {
                    case JsonType.Object:
                        if (value.Object != null)
                        {
                            ObjectBegin();
                            foreach (var pair in value.Object)
                            {
                                Key(pair.Key);
                                Json(pair.Value);
                            }
                            ObjectEnd();
                            return;
                        }
                        break;

                    case JsonType.Array:
                        if (value.Array != null)
                        {
                            ArrayBegin();
                            foreach (var item in value.Array)
                                Json(item);
                            ArrayEnd();
                            return;
                        }
                        break;

                    case JsonType.Bool:
                        writer.Write(value.Bool);
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
                }
            }

            Null();
        }

        private bool StringContainsAny(string value, string chars)
        {
            for (int i = 0; i < chars.Length; i++)
                if (value.Contains(chars[i]))
                    return true;
            return false;
        }

        private void EscapedString(string value)
        {
            bool encapsulate = Strict || StringContainsAny(value, ":#{}[],\"\n\r") || (value.Length > 0 && value[0] == ' ');

            if (encapsulate)
            {
                writer.Write('\"');

                for (int i = 0; i < value.Length; i++)
                {
                    switch (value[i])
                    {
                        case '\n': writer.Write("\\n"); break;
                        case '\t': writer.Write("\\t"); break;
                        case '\r': writer.Write("\\r"); break;
                        case '\"': writer.Write("\\\""); break;

                        default:
                            writer.Write(value[i]);
                            break;
                    }
                }

                writer.Write('\"');
            }
            else
            {
                writer.Write(value);
            }
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
