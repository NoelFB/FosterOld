using Foster.Framework;
using SDL2;
using System;

namespace Foster.SDL2
{
    internal class SDL_GLContext : ISystemOpenGL.Context
    {
        internal SDL_System system;
        internal IntPtr context;
        internal IntPtr window;
        internal bool disposed;

        internal SDL_GLContext(SDL_System system, IntPtr window)
        {
            this.system = system;
            this.window = window;
            context = SDL.SDL_GL_CreateContext(window);
            system.glContexts[context] = this;
        }

        public override bool IsDisposed => disposed;

        public override void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                SDL.SDL_GL_DeleteContext(context);
                system.glContexts.Remove(context);
                context = IntPtr.Zero;
            }
        }
    }
}
