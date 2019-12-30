namespace Foster.Framework
{
    /// <summary>
    /// Describes a Vertex Format for a Shader
    /// This tells the Shader what Attributes, and in what order, are to be expected
    /// </summary>
    public class VertexFormat
    {

        /// <summary>
        /// The list of Attributes
        /// </summary>
        public readonly VertexAttribute[] Attributes;

        /// <summary>
        /// The stride of each Vertex (all the Attributes combined)
        /// </summary>
        public readonly int Stride;

        public VertexFormat(params VertexAttribute[] attributes)
        {
            Attributes = attributes;

            Stride = 0;
            for (int i = 0; i < Attributes.Length; i++)
                Stride += Attributes[i].AttributeSize;
        }

        /// <summary>
        /// Attempts to find an attribute by name, and returns its relative pointer (offset)
        /// </summary>
        public bool TryGetAttribute(string name, out VertexAttribute element, out int pointer)
        {
            pointer = 0;
            for (int i = 0; i < Attributes.Length; i++)
            {
                if (Attributes[i].Name == name)
                {
                    element = Attributes[i];
                    return true;
                }
                pointer += Attributes[i].AttributeSize;
            }

            element = default;
            return false;
        }

    }
}
