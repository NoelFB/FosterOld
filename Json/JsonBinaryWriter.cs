using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Json
{
    public class JsonBinaryWriter : JsonWriter, IDisposable
    {
        public enum BinaryTokens
        {
            Null = 0,
            ObjectStart = 1,
            ObjectEnd = 2,
            ObjectKey = 3,
            ArrayStart = 4,
            ArrayEnd = 5,

            Boolean = 6,
            String = 7,
            Number = 8,

            Byte,
            Char,
            Short,
            UShort,
            Int,
            UInt,
            Long ,
            ULong,
            Decimal,
            Float,
            Double,

            Binary = 32,
            Comment = 64
        }

        private readonly BinaryWriter writer;
        private readonly Stack<long> containerPositionStack = new Stack<long>();

        public JsonBinaryWriter(string path)
        {
            writer = new BinaryWriter(File.Create(path), Encoding.UTF8);
        }

        public JsonBinaryWriter(Stream stream)
        {
            writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        private void ContainerBegin(BinaryTokens token)
        {
            writer.Write((byte)token);
            containerPositionStack.Push(writer.BaseStream.Position);
            writer.Write((uint)0); // byte size
        }

        private void ContainerEnd(BinaryTokens token)
        {
            writer.Write((byte)token);

            var current = writer.BaseStream.Position;
            var offset = containerPositionStack.Pop();

            writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            writer.Write((uint)(current - offset - 4)); // byte size (minus 4 since we want to skip the actual byte size value)
            writer.BaseStream.Seek(current, SeekOrigin.Begin);
        }

        public override void ObjectBegin()
        {
            ContainerBegin(BinaryTokens.ObjectStart);
        }

        public override void ObjectEnd()
        {
            ContainerEnd(BinaryTokens.ObjectEnd);
        }

        public override void ArrayBegin()
        {
            ContainerBegin(BinaryTokens.ArrayStart);
        }

        public override void ArrayEnd()
        {
            ContainerEnd(BinaryTokens.ArrayEnd);
        }

        public override void Comment(string text)
        {
            writer.Write((byte)BinaryTokens.Comment);
            writer.Write(text);
        }

        public override void Key(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Object Key cannot be empty");

            writer.Write((byte)BinaryTokens.ObjectKey);
            writer.Write(name);
        }

        public override void Null()
        {
            writer.Write((byte)BinaryTokens.Null);
        }

        public override void Value(bool value)
        {
            writer.Write((byte)BinaryTokens.Boolean);
            writer.Write(value);
        }

        public override void Value(byte value)
        {
            writer.Write((byte)BinaryTokens.Byte);
            writer.Write(value);
        }

        public override void Value(char value)
        {
            writer.Write((byte)BinaryTokens.Char);
            writer.Write(value);
        }

        public override void Value(short value)
        {
            writer.Write((byte)BinaryTokens.Short);
            writer.Write(value);
        }

        public override void Value(ushort value)
        {
            writer.Write((byte)BinaryTokens.UShort);
            writer.Write(value);
        }

        public override void Value(int value)
        {
            writer.Write((byte)BinaryTokens.Int);
            writer.Write(value);
        }

        public override void Value(uint value)
        {
            writer.Write((byte)BinaryTokens.UInt);
            writer.Write(value);
        }

        public override void Value(long value)
        {
            writer.Write((byte)BinaryTokens.Long);
            writer.Write(value);
        }

        public override void Value(ulong value)
        {
            writer.Write((byte)BinaryTokens.ULong);
            writer.Write(value);
        }

        public override void Value(decimal value)
        {
            writer.Write((byte)BinaryTokens.Decimal);
            writer.Write(value);
        }

        public override void Value(float value)
        {
            writer.Write((byte)BinaryTokens.Float);
            writer.Write(value);
        }

        public override void Value(double value)
        {
            writer.Write((byte)BinaryTokens.Double);
            writer.Write(value);
        }

        public override void Value(string value)
        {
            writer.Write((byte)BinaryTokens.String);
            writer.Write(value ?? "");
        }

        public override void Value(ReadOnlySpan<byte> value)
        {
            writer.Write((byte)BinaryTokens.Binary);
            writer.Write(value.Length);
            writer.Write(value);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
