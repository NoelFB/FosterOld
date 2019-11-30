using System;

namespace Foster.Framework
{
    public interface IProjectable2D
    {
        void Project(Vector2 axis, out float min, out float max);
    }

    public static class IProjectable2DExt
    {
        public static bool AxisOverlaps(this IProjectable2D a, IProjectable2D b, Vector2 axis, out float amount)
        {
            a.Project(axis, out float min0, out float max0);
            b.Project(axis, out float min1, out float max1);

            if (Math.Abs(min1 - max0) < Math.Abs(max1 - min0))
                amount = min1 - max0;
            else
                amount = max1 - min0;

            return (min0 < max1 && max0 > min1);
        }
    }
}
