using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Ray
    /// </summary>
    public struct Ray2D
    {

        /// <summary>
        /// Origin of the Ray
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Direction of the Ray
        /// </summary>
        public Vector2 Direction;

        public Ray2D(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }

        // TODO: implement intersection tests

    }
}
