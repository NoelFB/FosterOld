using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster
{
    public struct Facing.Framework
    {
        static public readonly Facing Right = new Facing(0);
        static public readonly Facing Left = new Facing(1);

        private byte value;

        private Facing(byte val)
        {
            value = val;
        }

        public Facing Reverse => new Facing((byte)(value ^ 1));

        static public implicit operator Facing(int v) => v < 0 ? Left : Right;
        static public implicit operator int(Facing f) => f.value == 0 ? 1 : -1;
        static public bool operator ==(Facing a, Facing b) => a.value == b.value;
        static public bool operator !=(Facing a, Facing b) => a.value != b.value;

        static public Facing operator *(Facing a, int b)
        {
            if (b < 0)
                return a.Reverse;
            else
                return a;
        }

        public override int GetHashCode()
        {
            return value;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Facing f))
                return false;
            else
                return this == f;
        }

        public byte ToByte()
        {
            return value;
        }

        public static Facing FromByte(byte v)
        {
            Debug.Assert(v < 2, "Argument out of range");
            return new Facing(v);
        }
    }
}
