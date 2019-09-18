namespace Foster.Framework
{
    public interface IProjectable3D
    {
        void Project(Vector3 axis, out float min, out float max);
    }
}
