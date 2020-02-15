using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;
using SDL2;

namespace Foster.SDL2
{
    public class SDL_System : Framework.System
    {
        public override bool SupportsMultipleWindows => true;

        public override Input Input => throw new NotImplementedException();

        protected override void ApplicationStarted()
        {
            throw new NotImplementedException("SDL2 System hasn't been implemented yet");
        }

        protected override Window.Platform CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            throw new NotImplementedException();
        }
    }
}
