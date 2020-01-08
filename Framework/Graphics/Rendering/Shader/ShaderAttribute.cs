namespace Foster.Framework
{
    /// <summary>
    /// A Shader Attribute
    /// </summary>
    public class ShaderAttribute
    {
        /// <summary>
        /// The name of the Attribute
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The Location of the Attribute in the Shader
        /// </summary>
        public readonly uint Location;

        public ShaderAttribute(string name, uint location)
        {
            Name = name;
            Location = location;
        }
    }
}
