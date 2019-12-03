namespace Foster.Framework
{
    public abstract class ShaderUniform
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
