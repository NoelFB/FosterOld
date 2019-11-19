using Foster.Framework;
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Foster.GLFW
{
    public class SDL_Input : Input
    {
        private readonly Stopwatch timer = new Stopwatch();

        protected override void Initialized()
        {
            timer.Start();

            // get API info
            {
                SDL.SDL_GetVersion(out var ver);

                ApiName = "SDL2";
                ApiVersion = new Version(ver.major, ver.minor, ver.patch);
            }

            base.Initialized();
        }

        public override void SetMouseCursor(Cursors cursors)
        {
            //throw new NotImplementedException();
        }

        public override ulong Timestamp()
        {
            return (ulong)timer.ElapsedMilliseconds;
        }
    }
}
