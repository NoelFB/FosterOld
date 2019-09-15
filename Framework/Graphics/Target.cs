using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public class Target : GraphicsResource
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool HasDepthBuffer { get; protected set; }

        public readonly ReadOnlyCollection<Texture> Attachments;
        protected readonly List<Texture> attachments = new List<Texture>();

        public Target(Graphics graphics) : base(graphics)
        {
            Attachments = attachments.AsReadOnly();
        }

        public static Target Create(int width, int height, int textures = 1, bool depthBuffer = false)
        {
            return App.GetModule<Graphics>().CreateTarget(width, height, textures, depthBuffer);
        }

        public static implicit operator Texture(Target target) => target.Attachments[0];
    }
}
