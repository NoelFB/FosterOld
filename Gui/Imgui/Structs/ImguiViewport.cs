using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public struct ImguiViewport
    {
        public ImguiID ID;
        public Batch2D Batcher;
        public Vector2 Scale;
        public Rect Bounds;
        public Vector2 Mouse;
        public Vector2 MouseDelta;
        public bool MouseObstructed;
        public ImguiID LastHotFrame;
        public ImguiID NextHotFrame;
        public int NextHotFrameLayer;
    }
}
