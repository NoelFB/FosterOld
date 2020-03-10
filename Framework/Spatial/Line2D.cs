using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Foster.Framework
{
    public struct Line2D : IConvexShape2D
    {
        public Vector2 From;
        public Vector2 To;

        public int Points => 2;
        public int Axis => 1;

        public Line2D(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }

        public Vector2 GetAxis(int index)
        {
            var axis = (To - From).Normalized();
            return new Vector2(axis.Y, -axis.X);
        }

        public Vector2 GetPoint(int index)
        {
            return index switch
            {
                0 => From,
                1 => To,
                _ => throw new IndexOutOfRangeException()
            };
        }

        public void Project(in Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            var dot = From.X * axis.X + From.Y * axis.Y;
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = To.X * axis.X + To.Y * axis.Y;
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }
    }
}
