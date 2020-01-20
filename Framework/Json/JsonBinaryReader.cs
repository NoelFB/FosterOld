using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    public class JsonBinaryReader : JsonReader, IDisposable
    {

        private readonly BinaryReader reader;

        public JsonBinaryReader(string path) : this(File.OpenRead(path))
        {

        }

        public JsonBinaryReader(Stream stream) : this(new BinaryReader(stream))
        {

        }

        public JsonBinaryReader(BinaryReader reader)
        {
            this.reader = reader;
        }

        public override bool Read()
        {
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var token = (JsonBinaryWriter.BinaryTokens)reader.ReadByte();
                
                switch (token)
                {
                    case JsonBinaryWriter.BinaryTokens.Null:
                        Value = null;
                        Token = JsonToken.Null;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ObjectStart:
                        Value = null;
                        Token = JsonToken.ObjectStart;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ObjectEnd:
                        Value = null;
                        Token = JsonToken.ObjectEnd;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ObjectKey:
                        Value = reader.ReadString();
                        Token = JsonToken.ObjectKey;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ArrayStart:
                        Value = null;
                        Token = JsonToken.ArrayStart;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ArrayEnd:
                        Value = null;
                        Token = JsonToken.ArrayEnd;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Boolean:
                        Value = reader.ReadBoolean();
                        Token = JsonToken.Boolean;
                        break;
                    case JsonBinaryWriter.BinaryTokens.String:
                        Value = reader.ReadString();
                        Token = JsonToken.String;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Byte:
                        Value = reader.ReadByte();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Char:
                        Value = reader.ReadChar();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Short:
                        Value = reader.ReadInt16();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.UShort:
                        Value = reader.ReadUInt16();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Int:
                        Value = reader.ReadInt32();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.UInt:
                        Value = reader.ReadUInt32();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Long:
                        Value = reader.ReadInt64();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.ULong:
                        Value = reader.ReadUInt64();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Decimal:
                        Value = reader.ReadDecimal();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Float:
                        Value = reader.ReadSingle();
                        Token = JsonToken.Number;
                        break;
                    case JsonBinaryWriter.BinaryTokens.Double:
                        Value = reader.ReadDouble();
                        Token = JsonToken.Number;
                        break;
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
