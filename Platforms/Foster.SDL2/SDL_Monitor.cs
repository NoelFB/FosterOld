using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GLFW
{
    public class SDL_Monitor : Monitor
    {
        public readonly IntPtr Pointer;

        private bool isPrimary;
        private RectInt bounds;
        private Vector2 contentScale;

        public override string Name { get; }
        public override bool IsPrimary => isPrimary;
        public override RectInt Bounds => bounds;
        public override Vector2 ContentScale => contentScale;

        public SDL_Monitor(IntPtr pointer)
        {
            Pointer = pointer;
            Name = "";
        }
    }
}
