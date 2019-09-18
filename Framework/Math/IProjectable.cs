namespace Foster.Framework
{
    public interface IProjectable
    {
        void Project(Vector2 axis, out float min, out float max);
    }
}
