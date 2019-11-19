using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GLFW
{
    public class GLFW_Monitor : Monitor
    {
        public readonly IntPtr Pointer;

        private bool isPrimary;
        private RectInt bounds;
        private Vector2 contentScale;

        public override string Name { get; }
        public override bool IsPrimary => isPrimary;
        public override RectInt Bounds => bounds;
        public override Vector2 ContentScale => contentScale;

        public GLFW_Monitor(IntPtr pointer)
        {
            Pointer = pointer;
            Name = GLFW.GetMonitorName(Pointer);

            Update();
        }

        public void Update()
        {
            GLFW.GetMonitorContentScale(Pointer, out contentScale.X, out contentScale.Y);
            GLFW.GetMonitorWorkarea(Pointer, out bounds.X, out bounds.Y, out bounds.Width, out bounds.Height);
            isPrimary = GLFW.GetPrimaryMonitor() == Pointer;
        }
    }
}
