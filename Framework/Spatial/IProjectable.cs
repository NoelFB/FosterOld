namespace Foster.Framework
{
    public interface IProjectable
    {
        void Project(Vector3 axis, out float min, out float max);
    }
}
