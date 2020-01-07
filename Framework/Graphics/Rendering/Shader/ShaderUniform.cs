namespace Foster.Framework
{
    public class ShaderUniform
    {
        /// <summary>
        /// The Name of the Uniform
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The Location of the Uniform in the Shader
        /// </summary>
        public readonly int Location;

        /// <summary>
        /// The size of the uniform (ex. Array Length)
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The Type of Uniform
        /// </summary>
        public readonly UniformType Type;

        public ShaderUniform(string name, int location, int size, UniformType type)
        {
            Name = name;
            Location = location;
            Size = size;
            Type = type;
        }
    }
}
