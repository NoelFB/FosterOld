using System;
using System.IO;

namespace Foster.Json
{
    /// <summary>
    /// Writes Json to a string format
    /// </summary>
    public class JsonTextWriter : JsonWriter, IDisposable
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

        public JsonTextWriter(string path, bool strict = true) : this(File.Create(path), strict)
        {
        }

        public JsonTextWriter(Stream stream, bool strict = true) : this(new StreamWriter(stream), strict)
        {
        }

        public JsonTextWriter(TextWriter writer, bool strict = true)
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

        public override void Key(string name)
        {
            Next(isKey: true);
            EscapedString(name);

            writer.Write(":");
            if (Verbose)
                writer.Write(" ");
        }

        public override void ObjectBegin()
        {
            ContainerBegin('{');
        }

        public override void ObjectEnd()
        {
            ContainerEnd('}');
        }

        public override void ArrayBegin()
        {
            ContainerBegin('[');
        }

        public override void ArrayEnd()
        {
            ContainerEnd(']');
        }

        public override void Comment(string text)
        {
            if (!Strict && Verbose && text.Length > 0)
            {
                ReadOnlySpan<char> span = text;
                int last = 0;
                int next;
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

        public override void Null()
        {
            Next(isValue: true);
            writer.Write("null");
        }

        public override void Value(bool value)
        {
            Next(isValue: true);
            writer.Write(value ? "true" : "false");
        }

        public override void Value(byte value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(char value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(short value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(ushort value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(int value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(uint value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(long value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(ulong value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(decimal value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(float value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(double value)
        {
            Next(isValue: true);
            writer.Write(value);
        }

        public override void Value(string value)
        {
            Next(isValue: true);
            EscapedString(value);
        }

        public override void Value(ReadOnlySpan<byte> value)
        {
            Next(isValue: true);
            writer.Write('"');
            writer.Write("bin::");
            writer.Write(Convert.ToBase64String(value, Base64FormattingOptions.None));
            writer.Write('"');
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
            bool encapsulate = Strict || StringContainsAny(value, ":#{}[],\"\n\r") || (value.Length > 0 && char.IsWhiteSpace(value[0])) || value.Length <= 0;

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
                        case '\\': writer.Write("\\\\"); break;

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
