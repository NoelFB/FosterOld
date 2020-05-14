using System;
using System.IO;
using System.Text;

namespace Foster.Json
{
    /// <summary>
    /// Reads JSON from a Stream or Path
    /// </summary>
    public class JsonTextReader : JsonReader, IDisposable
    {

        private readonly TextReader reader;
        private readonly StringBuilder builder = new StringBuilder();
        private readonly bool disposeStream;

        // in the case where the value of a previous key is completely empty, we want to
        // return null, and then store the current value for the next Read call
        // this only matters for non-strict JSON
        private bool storedNext;
        private string? storedString;
        private JsonToken storedToken;
        private long position;

        public JsonTextReader(string path) : this(File.OpenRead(path))
        {

        }

        public JsonTextReader(Stream stream, bool disposeStream = true) : this(new StreamReader(stream, Encoding.UTF8, true, 4096), disposeStream)
        {

        }

        public JsonTextReader(TextReader reader, bool disposeStream = true)
        {
            this.reader = reader;
            this.disposeStream = disposeStream;
            position = 0;
        }

        public override long Position => position;

        public override bool Read()
        {
            Value = null;
            var lastToken = Token;

            if (storedNext)
            {
                Value = storedString;
                Token = storedToken;
                storedNext = false;
                return true;
            }

            while (StepChar(out var next))
            {
                // skip whitespace and characters we don't care about
                if (char.IsWhiteSpace(next) || next == ':' || next == ',')
                    continue;

                // a comment
                if (next == '#' || (next == '/' && (PeekChar(out var p) && p == '/')))
                {
                    while (StepChar(out next) && next != '\n' && next != '\r')
                        continue;
                    continue;
                }

                var isEncapsulated = false;

                switch (next)
                {
                    // object
                    case '{':
                        Token = JsonToken.ObjectStart;
                        return true;

                    case '}':

                        // if we found an object-end after a key
                        // set the value of that last key to null, and store this value for next time
                        if (lastToken == JsonToken.ObjectKey)
                        {
                            storedNext = true;
                            storedToken = JsonToken.ObjectEnd;
                            storedString = null;

                            Value = null;
                            Token = JsonToken.Null;
                            return true;
                        }

                        Token = JsonToken.ObjectEnd;
                        return true;

                    // array
                    case '[':
                        Token = JsonToken.ArrayStart;
                        return true;

                    case ']':

                        // if we found an array-end after a key
                        // set the value of that last key to null, and store this value for next time
                        if (lastToken == JsonToken.ObjectKey)
                        {
                            storedNext = true;
                            storedToken = JsonToken.ArrayEnd;
                            storedString = null;

                            Value = null;
                            Token = JsonToken.Null;
                            return true;
                        }

                        Token = JsonToken.ArrayEnd;
                        return true;

                    // an encapsulated string
                    case '"':
                        {
                            builder.Clear();

                            while (StepChar(out next) && next != '"')
                            {
                                if (next == '\\')
                                {
                                    StepChar(out next);
                                    if (next == 'n')
                                        builder.Append('\n');
                                    else if (next == 'r')
                                        builder.Append('\r');
                                    else
                                        builder.Append(next);
                                    continue;
                                }

                                builder.Append(next);
                            }

                            isEncapsulated = true;
                            break;
                        }

                    // other value
                    default:
                        {
                            builder.Clear();
                            builder.Append(next);

                            while (PeekChar(out next) && !("\r\n,:{}[]#").Contains(next))
                            {
                                builder.Append(next);
                                SkipChar();
                            }

                            break;
                        }
                }

                // check if this entry is a KEY
                bool isKey = false;
                {
                    if (char.IsWhiteSpace(next))
                    {
                        while (PeekChar(out next) && char.IsWhiteSpace(next))
                            SkipChar();
                    }

                    if (PeekChar(out next) && next == ':')
                        isKey = true;
                }

                // is a key
                if (isKey)
                {
                    // if we found an key after a key
                    // set the value of that last key to null, and store this value for next time
                    if (lastToken == JsonToken.ObjectKey)
                    {
                        storedNext = true;
                        storedToken = JsonToken.ObjectKey;
                        storedString = builder.ToString();

                        Value = null;
                        Token = JsonToken.Null;
                        return true;
                    }

                    Token = JsonToken.ObjectKey;
                    Value = builder.ToString();
                    return true;
                }
                // is an ecnapsulated string
                else if (isEncapsulated)
                {
                    var str = builder.ToString();

                    if (str.StartsWith("bin::"))
                    {
                        Token = JsonToken.Binary;
                        Value = Convert.FromBase64String(str.Substring(5));
                        return true;
                    }
                    else
                    {
                        Token = JsonToken.String;
                        Value = str;
                        return true;
                    }
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

                        // decimal, float, double
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
                        else if (ulong.TryParse(str, out ulong ulongValue))
                        {
                            Value = ulongValue;
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

            bool SkipChar()
            {
                return StepChar(out _);
            }

            bool StepChar(out char next)
            {
                int read = reader.Read();
                next = (char)read;
                position++;
                return read >= 0;
            }

            bool PeekChar(out char next)
            {
                int read = reader.Peek();
                next = (char)read;
                return read >= 0;
            }
        }

        public void Dispose()
        {
            if (disposeStream)
                reader.Dispose();
        }
    }
}
