using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public struct Cardinal
    {
        static public readonly Cardinal Null = new Cardinal(0, 0);
        static public readonly Cardinal Right = new Cardinal(1, 0);
        static public readonly Cardinal Left = new Cardinal(-1, 0);
        static public readonly Cardinal Up = new Cardinal(0, -1);
        static public readonly Cardinal Down = new Cardinal(0, 1);

        public int X { get; private set; }
        public int Y { get; private set; }

        private Cardinal(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Cardinal Reverse => new Cardinal(-X, -Y);
        public Cardinal TurnRight => new Cardinal(-Y, X);
        public Cardinal TurnLeft => new Cardinal(Y, -X);

        public float Angle
        {
            get
            {
                if (this == Cardinal.Left)
                    return Calc.PI;
                else if (this == Cardinal.Up)
                    return -Calc.HalfPI;
                else if (this == Cardinal.Down)
                    return Calc.HalfPI;
                else
                    return 0;
            }
        }

        static public implicit operator Point2(Cardinal c) => new Point2(c.X, c.Y);
        static public bool operator ==(Cardinal a, Cardinal b) => a.X == b.X && a.Y == b.Y;
        static public bool operator !=(Cardinal a, Cardinal b) => a.X != b.X || a.Y != b.Y;

        public byte ToByte()
        {
            if (this == Null)
                return 0;
            else if (this == Right)
                return 1;
            else if (this == Up)
                return 2;
            else if (this == Left)
                return 3;
            else if (this == Down)
                return 4;

            throw new ArgumentOutOfRangeException();
        }

        public static Cardinal FromByte(byte v)
        {
            switch (v)
            {
                case 0:
                    return Null;
                case 1:
                    return Right;
                case 2:
                    return Up;
                case 3:
                    return Left;
                case 4:
                    return Down;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
