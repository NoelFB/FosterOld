using Foster.Framework;
using SDL2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Foster.SDL2
{
    public class SDL_Context : Context
    {
        internal IntPtr Pointer { get; private set; } = IntPtr.Zero;
        internal readonly IntPtr Window = IntPtr.Zero;

        public override Framework.System System { get; }

        public override bool Disposed => Pointer == IntPtr.Zero;

        public override int Width
        {
            get
            {
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    SDL.SDL_GL_GetDrawableSize(Window, out int w, out _);
                    return w;
                }

                throw new NotImplementedException();
            }
        }

        public override int Height
        {
            get
            {
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    SDL.SDL_GL_GetDrawableSize(Window, out _, out int h);
                    return h;
                }

                throw new NotImplementedException();
            }
        }

        public SDL_Context(SDL_System system, IntPtr window)
        {
            System = system;
            Window = window;

            system.contexts.Add(this);

            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                Pointer = SDL.SDL_GL_CreateContext(window);
                if (Pointer == IntPtr.Zero)
                    throw new Exception($"SDL Error: {SDL.SDL_GetError()}");
            }
        }

        public override void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                SDL.SDL_GL_DeleteContext(Pointer);

                if (System is SDL_System sys)
                    sys.contexts.Remove(this);
            }

            Pointer = IntPtr.Zero;
        }
    }
}
