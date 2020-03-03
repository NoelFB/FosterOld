using Foster.Framework;
using SDL2;
using System;
using System.Numerics;

namespace Foster.SDL2
{
    internal class SDL_Monitor : Monitor
    {
        public readonly int Index;

        private string name;
        private RectInt bounds;
        private Vector2 contentScale;

        public override string Name => name;
        public override bool IsPrimary => Index == 0;
        public override RectInt Bounds => bounds;
        public override Vector2 ContentScale => contentScale;

        public SDL_Monitor(int index)
        {
            Index = index;

            name = SDL.SDL_GetDisplayName(index);
            FetchProperties();
        }

        public void FetchProperties()
        {
            SDL.SDL_GetDisplayBounds(Index, out var rect);
            bounds = new RectInt(rect.x, rect.y, rect.w, rect.h);

            float hidpiRes = 72f;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                hidpiRes = 96;
            SDL.SDL_GetDisplayDPI(Index, out float ddpi, out _, out _);
            contentScale = Vector2.One * (ddpi / hidpiRes);
        }
    }
}
