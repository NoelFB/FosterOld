namespace Foster.Framework
{
    public class ShaderUniform
    {
        public enum Types
        {
            Unknown,

            Int,

            Float,
            Float2,
            Float3,
            Float4,

            Matrix2D,
            Matrix,

            Texture2D
        }

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
        public readonly Types Type;

        public ShaderUniform(string name, int location, int size, Types type)
        {
            Name = name;
            Location = location;
            Size = size;
            Type = type;
        }
    }
}
