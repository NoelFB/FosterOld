using System.Runtime.InteropServices;

namespace Foster.Framework
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2D
    {
        [VertexAttribute(0, VertexType.Float, 2)]
        public Vector2 Pos;

        [VertexAttribute(1, VertexType.Float, 2)]
        public Vector2 Tex;

        [VertexAttribute(2, VertexType.UnsignedByte, 4, true)]
        public Color Col;

        [VertexAttribute(3, VertexType.UnsignedByte, 1, true)]
        public byte Mult;

        [VertexAttribute(4, VertexType.UnsignedByte, 1, true)]
        public byte Wash;

        [VertexAttribute(5, VertexType.UnsignedByte, 1, true)]
        public byte Fill;

        public Vertex2D(Vector2 position, Vector2 texcoord, Color color, int mult, int wash, int fill)
        {
            Pos = position;
            Tex = texcoord;
            Col = color;
            Mult = (byte)mult;
            Wash = (byte)wash;
            Fill = (byte)fill;
        }

        public override string ToString()
        {
            return $"{{Pos:{Pos}, Tex:{Tex}, Col:{Col}, Mult:{Mult}, Wash:{Wash}, Fill:{Fill}}}";
        }

    }
}