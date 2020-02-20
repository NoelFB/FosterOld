using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Foster.Framework
{
    public static class VectorExt
    {
        /// <summary>
        /// Floors the individual components of a Vector2
        /// </summary>
        public static Vector2 Floor(this Vector2 vector) => new Vector2(MathF.Floor(vector.X), MathF.Floor(vector.Y));

        /// <summary>
        /// Floors the individual components of a Vector3
        /// </summary>
        public static Vector3 Floor(this Vector3 vector) => new Vector3(MathF.Floor(vector.X), MathF.Floor(vector.Y), MathF.Floor(vector.Z));

        /// <summary>
        /// Floors the individual components of a Vector4
        /// </summary>
        public static Vector4 Floor(this Vector4 vector) => new Vector4(MathF.Floor(vector.X), MathF.Floor(vector.Y), MathF.Floor(vector.Z), MathF.Floor(vector.W));

        /// <summary>
        /// Turns a Vector2 to its right perpendicular
        /// </summary>
        public static Vector2 TurnRight(this Vector2 vector) => new Vector2(-vector.Y, vector.X);

        /// <summary>
        /// Turns a Vector2 to its left perpendicular
        /// </summary>
        public static Vector2 TurnLeft(this Vector2 vector) => new Vector2(vector.Y, -vector.X);

    }
}
