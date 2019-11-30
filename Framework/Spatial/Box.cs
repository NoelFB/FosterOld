using System;

namespace Foster.Framework
{
    public struct Box : IProjectable
    {

        public Vector3 Position;
        public Vector3 Size;

        public float Left => Position.X - Size.X / 2;
        public float Right => Position.X + Size.X / 2;
        public float Top => Position.Y - Size.Y / 2;
        public float Bottom => Position.Y + Size.Y / 2;
        public float Front => Position.Z - Size.Z / 2;
        public float Back => Position.Z + Size.Z / 2;

        public void Project(Vector3 axis, out float min, out float max)
        {
            throw new NotImplementedException();
        }
    }
}
