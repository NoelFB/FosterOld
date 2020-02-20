using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A 3D Ray
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// Origin of the Ray
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Direction of the Ray
        /// </summary>
        public Vector3 Direction;

        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        // TODO: implement intersection tests

    }
}
