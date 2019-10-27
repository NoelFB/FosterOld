namespace Foster.Framework
{
    public abstract class ShaderUniform
    {
        public int Location { get; protected set; } = 0;
        public string Name { get; protected set; } = "";
        public UniformType Type { get; protected set; } = UniformType.Unknown;
    }
}
