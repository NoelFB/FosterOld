namespace Foster.Framework
{
    public abstract class ShaderAttribute
    {
        /// <summary>
        /// The name of the Attribute
        /// </summary>
        public string Name { get; protected set; } = "";

        /// <summary>
        /// The Location of the Attribute in the Shader
        /// </summary>
        public uint Location { get; protected set; } = 0;
    }
}
