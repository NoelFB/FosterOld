using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Target : GraphicsResource
    {
        public readonly int Width;
        public readonly int Height;

        protected List<Texture> attachments = new List<Texture>();

        public Target(Graphics graphics) : base(graphics)
        {

        }

        public Texture this[int index] => attachments[index];

        public int Attachments => attachments.Count;
    }
}
