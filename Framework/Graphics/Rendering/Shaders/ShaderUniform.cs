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
        public string Name { get; protected set; }

        /// <summary>
        /// The Location of the Uniform in the Shader
        /// </summary>
        public int Location { get; protected set; }

        /// <summary>
        /// The size of the uniform (ex. Array Length)
        /// </summary>
        public int Size { get; protected set; }

        /// <summary>
        /// The Type of Uniform
        /// </summary>
        public Types Type { get; protected set; }

        public ShaderUniform(string name, int location, int size, Types type)
        {
            Name = name;
            Location = location;
            Size = size;
            Type = type;
        }
    }
}
