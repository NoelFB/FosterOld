using System.Runtime.InteropServices;

namespace Foster.Framework
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2D
    {
        public enum Modes
        {
            Multiply,
            Wash,
            Fill
        }

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

        public Modes Mode
        {
            get => (Mult > 0 ? Modes.Multiply : (Wash > 0 ? Modes.Wash : Modes.Fill));
            set
            {
                Mult = (byte)(value == Modes.Multiply ? 255 : 0);
                Wash = (byte)(value == Modes.Wash ? 255 : 0);
                Fill = (byte)(value == Modes.Fill ? 255 : 0);
            }
        }

        public Vertex2D(Vector2 position, Vector2 texcoord, Color color, Modes mode = Modes.Multiply)
        {
            Pos = position;
            Tex = texcoord;
            Col = color;
            Mult = (byte)(mode == Modes.Multiply ? 255 : 0);
            Wash = (byte)(mode == Modes.Wash ? 255 : 0);
            Fill = (byte)(mode == Modes.Fill ? 255 : 0);
        }

        public Vertex2D(Vector2 position, Vector2 texcoord, Color color, byte mult, byte wash, byte fill)
        {
            Pos = position;
            Tex = texcoord;
            Col = color;
            Mult = mult;
            Wash = wash;
            Fill = fill;
        }

        public override string ToString()
        {
            return $"{{Position:{Pos}, Texcoord:{Tex}, Color:{Col}, Mode:{Mode}}}";
        }

    }
}