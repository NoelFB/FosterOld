using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public unsafe struct ConvexPolygon2D : IConvexShape2D
    {
        private fixed float points[64];

        public int Points;
        public int Sides => Points;

        public Vector2 GetPoint(int index)
        {
            return new Vector2(
                points[index * 2 + 0], 
                points[index * 2 + 1]);
        }

        public void SetPoint(int index, Vector2 position)
        {
            points[index * 2 + 0] = position.X;
            points[index * 2 + 1] = position.Y;
        }

        public Vector2 this[int index]
        {
            get => GetPoint(index);
            set => SetPoint(index, value);
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            if (Points <= 0)
            {
                min = max = 0;
            }
            else
            {
                min = float.MaxValue;
                max = float.MinValue;

                for (int i = 0; i < Points; i++)
                {
                    var dot = Vector2.Dot(new Vector2(points[i * 2 + 0], points[i * 2 + 1]), axis);
                    min = Math.Min(dot, min);
                    max = Math.Max(dot, max);
                }
            }
        }
    }
}
