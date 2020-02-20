using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// a 3D shape that can be projected onto an Axis
    /// </summary>
    public interface IProjectable
    {
        void Project(Vector3 axis, out float min, out float max);
    }
}
