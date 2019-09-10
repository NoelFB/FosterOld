using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public class Target : GraphicsResource
    {
        public readonly int Width;
        public readonly int Height;
        public readonly ReadOnlyCollection<Texture> Attachments;

        protected readonly List<Texture> attachments = new List<Texture>();

        public Target(Graphics graphics) : base(graphics)
        {
            Attachments = attachments.AsReadOnly();
        }
    }
}
