namespace Foster.Framework
{
    public abstract class ShaderUniform
    {
        public enum Types
        {
            Unknown = 0,

            Int = 0x100,
            Int2,
            Int3,
            Int4,

            Float = 0x200,
            Float2,
            Float3,
            Float4,

            Matrix2D = 0x300,
            Matrix,

            Texture2D = 0x400
        }

        /// <summary>
        /// The Name of the Uniform
        /// </summary>
        public string Name { get; protected set; } = "";

        /// <summary>
        /// The Location of the Uniform in the Shader
        /// </summary>
        public int Location { get; protected set; } = 0;

        /// <summary>
        /// The Type of Uniform
        /// </summary>
        public Types Type { get; protected set; } = Types.Unknown;
    }
}
