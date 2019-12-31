using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
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
