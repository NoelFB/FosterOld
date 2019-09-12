using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        private byte multiply;

        [VertexAttribute(4, VertexType.UnsignedByte, 1, true)]
        private byte wash;

        [VertexAttribute(5, VertexType.UnsignedByte, 1, true)]
        private byte fill;

        public Modes Mode
        {
            get => (multiply > 0 ? Modes.Multiply : (wash > 0 ? Modes.Wash : Modes.Fill));
            set
            {
                multiply = (byte)(value == Modes.Multiply ? 255 : 0);
                wash = (byte)(value == Modes.Wash ? 255 : 0);
                fill = (byte)(value == Modes.Fill ? 255 : 0);
            }
        }

        public Vertex2D(Vector2 position, Vector2 texcoord, Color color, Modes mode = Modes.Multiply)
        {
            Pos = position;
            Tex = texcoord;
            Col = color;
            multiply = (byte)(mode == Modes.Multiply ? 255 : 0);
            wash = (byte)(mode == Modes.Wash ? 255 : 0);
            fill = (byte)(mode == Modes.Fill ? 255 : 0);
        }

        public override string ToString()
        {
            return $"{{Position:{Pos}, Texcoord:{Tex}, Color:{Col}, Mode:{Mode}}}";
        }

    }
}