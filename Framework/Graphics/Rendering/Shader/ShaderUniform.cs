namespace Foster.Framework
{
    /// <summary>
    /// A Shader Uniform Value
    /// </summary>
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
        /// The Array length of the uniform
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The Type of Uniform
        /// </summary>
        public readonly UniformType Type;

        public ShaderUniform(string name, int location, int length, UniformType type)
        {
            Name = name;
            Location = location;
            Length = length;
            Type = type;
        }
    }
}
