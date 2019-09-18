namespace Foster.Framework
{
    public struct Box
    {

        public Vector3 Position;
        public Vector3 Size;

        public Vector3 Center => Position + Size / 2f;
        public float Left => Position.X;
        public float Right => Position.X + Size.X;
        public float Top => Position.Y;
        public float Bottom => Position.Y + Size.Y;
        public float Front => Position.Z;
        public float Back => Position.Z + Size.Z;

    }
}
