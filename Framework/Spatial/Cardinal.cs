using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Foster.Framework
{
    public struct Cardinal
    {
        static public readonly Cardinal Right = new Cardinal(0);
        static public readonly Cardinal Up = new Cardinal(1);
        static public readonly Cardinal Left = new Cardinal(2);
        static public readonly Cardinal Down = new Cardinal(3);

        private byte value;

        private Cardinal(byte val)
        {
            value = val;
        }

        public Cardinal Reverse => new Cardinal((byte)((value + 2) % 4));
        public Cardinal TurnRight => new Cardinal((byte)((value + 3) % 4));
        public Cardinal TurnLeft => new Cardinal((byte)((value + 1) % 4));

        public int X
        {
            get
            {
                switch (value)
                {
                    case 0:
                        return 1;
                    case 2:
                        return -1;
                    case 1:
                    case 3:
                        return 0;
                }

                throw new ArgumentException();
            }
        }

        public int Y
        {
            get
            {
                switch (value)
                {
                    case 3:
                        return 1;
                    case 1:
                        return -1;
                    case 0:
                    case 2:
                        return 0;
                }

                throw new ArgumentException();
            }
        }

        public float Angle
        {
            get
            {
                switch (value)
                {
                    case 0:
                        return 0;
                    case 1:
                        return -Calc.HalfPI;
                    case 2:
                        return Calc.PI;
                    case 3:
                        return Calc.HalfPI;
                }

                throw new ArgumentException();
            }
        }

        static public implicit operator Cardinal(Facing f) => f.ToByte() == 0 ? Cardinal.Right : Cardinal.Left;
        static public implicit operator Point2(Cardinal c) => new Point2(c.X, c.Y);
        static public bool operator ==(Cardinal a, Cardinal b) => a.value == b.value;
        static public bool operator !=(Cardinal a, Cardinal b) => a.value != b.value;
        static public Point2 operator *(Cardinal a, int b) => (Point2)a * b;

        public override int GetHashCode()
        {
            return value;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Cardinal c))
                return false;
            else
                return this == c;
        }

        public override string ToString()
        {
            switch (value)
            {
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                default: return "Down";
            }
        }

        public byte ToByte()
        {
            return value;
        }

        public static Cardinal FromByte(byte v)
        {
            Debug.Assert(v < 4, "Argument out of range");
            return new Cardinal(v);
        }

        public static Cardinal FromVector(Vector2 dir)
        {
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
                return dir.X < 0 ? Left : Right;
            return dir.Y < 0 ? Up : Down;
        }

        public static IEnumerable<Cardinal> All
        {
            get
            {
                yield return Right;
                yield return Down;
                yield return Left;
                yield return Up;
            }
        }
    }
}
