using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework.Json
{
    public class JsonReader : IDisposable
    {

        public JsonToken Token { get; private set; }
        public object? Value { get; private set; }

        private readonly TextReader reader;
        private readonly StringBuilder builder = new StringBuilder();

        private int line = 1;
        private int index;

        public JsonReader(string path) : this(File.OpenRead(path))
        {

        }

        public JsonReader(Stream stream) : this(new StreamReader(stream, Encoding.UTF8, true, 4096))
        {

        }

        public JsonReader(TextReader reader)
        {
            this.reader = reader;
        }

        public JsonObject ReadObject()
        {
            var obj = new JsonObject();
            var opened = false;

            while (Read() && Token != JsonToken.ObjectEnd)
            {
                if (!opened && Token == JsonToken.ObjectStart)
                {
                    opened = true;
                    continue;
                }

                if (Token != JsonToken.ObjectKey)
                    throw new Exception($"Expected Object Key at line {line}, index {index}");

                var key = Value as string;
                if (string.IsNullOrEmpty(key))
                    throw new Exception($"Invalid Object Key {line}, index {index}");

                obj[key] = ReadValue();
            }

            return obj;
        }

        public bool TryReadObject([MaybeNullWhen(false)] out JsonObject obj)
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

        public JsonArray ReadArray()
        {
            var arr = new JsonArray();
            while (Read() && Token != JsonToken.ArrayEnd)
                arr.Add(CurrentValue());
            return arr;
        }

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
                    if (Value is bool b)
                        return b;
                    break;

                case JsonToken.Number:
                    if (Value is float f) 
                        return f;
                    if (Value is double d) 
                        return d;
                    if (Value is int i)
                        return i;
                    if (Value is long l)
                        return l;
                    break;

                case JsonToken.String:
                    if (Value is string s)
                        return s;
                    break;

                case JsonToken.ObjectStart:
                    return ReadObject();

                case JsonToken.ArrayStart:
                    return ReadArray();

                case JsonToken.ObjectKey:
                    throw new Exception($"Unexpected Object Key at line {line}, index {index}");

                case JsonToken.ObjectEnd:
                    throw new Exception($"Unexpected Object End at line {line}, index {index}");

                case JsonToken.ArrayEnd:
                    throw new Exception($"Unexpected Array End at line {line}, index {index}");
            }

            return new JsonNull();
        }

        public bool Read()
        {
            Value = null;

            while (Step(out var next))
            {
                // skip whitespace and characters we don't care about
                if (char.IsWhiteSpace(next) || next == ':' || next == ',')
                    continue;

                var isEncapsulated = false;

                switch (next)
                {
                    // object
                    case '{':
                        Token = JsonToken.ObjectStart;
                        return true;
                    case '}':
                        Token = JsonToken.ObjectEnd;
                        return true;

                    // array
                    case '[':
                        Token = JsonToken.ArrayStart;
                        return true;
                    case ']':
                        Token = JsonToken.ArrayEnd;
                        return true;

                    // an encapsulated string
                    case '"':
                        {
                            builder.Clear();

                            char last = next;
                            while (Step(out next) && (next != '"' || last == '\\'))
                                builder.Append(last = next);

                            isEncapsulated = true;
                            break;
                        }

                    // other value
                    default:
                        {
                            builder.Clear();
                            builder.Append(next);

                            while (Peek(out next) && !("\r\n,:{}[]#").Contains(next))
                            {
                                builder.Append(next);
                                Skip();
                            }

                            break;
                        }
                }

                // check if this entry is a KEY
                bool isKey = false;
                {
                    if (char.IsWhiteSpace(next))
                    {
                        while (Peek(out next) && char.IsWhiteSpace(next))
                            Skip();
                    }

                    if (Peek(out next) && next == ':')
                        isKey = true;
                }

                // is a key
                if (isKey)
                {
                    Token = JsonToken.ObjectKey;
                    Value = builder.ToString();
                    return true;
                }
                // is an ecnapsulated string
                else if (isEncapsulated)
                {
                    Token = JsonToken.String;
                    Value = builder.ToString();
                    return true;
                }
                else
                {
                    var str = builder.ToString();

                    // null value
                    if (str.Length <= 0 || str.Equals("null", StringComparison.OrdinalIgnoreCase))
                    {
                        Token = JsonToken.Null;
                        return true;
                    }
                    // true value
                    else if (str.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        Token = JsonToken.Boolean;
                        Value = true;
                        return true;
                    }
                    // false value
                    else if (str.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        Token = JsonToken.Boolean;
                        Value = false;
                        return true;
                    }
                    // could be a number value ...
                    // this is kinda ugly ... but we just try to fit it into the smallest number type it can be
                    else if ((str[0] >= '0' && str[0] <= '9') || str[0] == '-' || str[0] == '+' || str[0] == '.')
                    {
                        Token = JsonToken.Number;

                        // float or double
                        if (str.Contains('.'))
                        {
                            if (float.TryParse(str, out float floatValue))
                            {
                                Value = floatValue;
                                return true;
                            }
                            else if (double.TryParse(str, out double doubleValue))
                            {
                                Value = doubleValue;
                                return true;
                            }
                        }
                        else if (int.TryParse(str, out int intValue))
                        {
                            Value = intValue;
                            return true;
                        }
                        else if (long.TryParse(str, out long longValue))
                        {
                            Value = longValue;
                            return true;
                        }
                    }

                    // fallback to string
                    Token = JsonToken.String;
                    Value = str;
                    return true;
                }

            }

            return false;

            bool Skip()
            {
                return Step(out _);
            }

            bool Step(out char next)
            {
                int read = reader.Read();
                next = (char)read;

                // keep track of line
                if (next == '\n')
                {
                    line++;
                    index = 0;
                }
                else
                    index++;

                return read >= 0;
            }

            bool Peek(out char next)
            {
                int read = reader.Peek();
                next = (char)read;
                return read >= 0;
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}