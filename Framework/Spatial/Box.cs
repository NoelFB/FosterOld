using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// A 3D Box
    /// </summary>
    public struct Box : IProjectable
    {

        public Vector3 Position;
        public Vector3 Size;

        public void Project(Vector3 axis, out float min, out float max)
        {
            throw new NotImplementedException();
        }
    }
}
