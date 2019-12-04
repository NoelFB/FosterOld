namespace Foster.Framework
{
    public class VertexFormat
    {

        public readonly VertexElement[] Elements;
        public readonly int Stride;

        public VertexFormat(params VertexElement[] elements)
        {
            Elements = elements;

            Stride = 0;
            for (int i = 0; i < Elements.Length; i++)
                Stride += Elements[i].ElementSizeInBytes;
        }

        public bool TryGetElement(string name, out VertexElement element, out int pointer)
        {
            pointer = 0;
            for (int i = 0; i < Elements.Length; i++)
                if (Elements[i].Name == name)
                {
                    element = Elements[i];
                    return true;
                }
                else
                    pointer += Elements[i].ElementSizeInBytes;

            element = default;
            return false;
        }

    }
}
