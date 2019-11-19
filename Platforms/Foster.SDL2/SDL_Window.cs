using Foster.Framework;
using SDL2;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Foster.SDL2
{
    public class SDL_Window : Window
    {
        private string title;
        private bool visible;
        private bool lastVsync;
        private bool bordered;
        private bool resizable;

        public override IntPtr Pointer { get; }
        public override Framework.System System { get; }
        public override Context Context { get; }

        public override Point2 Position
        {
            get
            {
                SDL.SDL_GetWindowPosition(Pointer, out int x, out int y);
                return new Point2(x, y);
            }
            set
            {
                SDL.SDL_SetWindowPosition(Pointer, value.X, value.Y);
            }
        }

        public override Point2 Size
        {
            get
            {
                SDL.SDL_GetWindowSize(Pointer, out int x, out int y);
                return new Point2(x, y);
            }
            set
            {
                SDL.SDL_SetWindowSize(Pointer, value.X, value.Y);
            }
        }

        public override Vector2 ContentScale
        {
            get
            {
                var display = SDL.SDL_GetWindowDisplayIndex(Pointer);
                SDL.SDL_GetDisplayDPI(display, out var ddpi, out var hdpi, out var vdpi);

                var hidpiRes = 72f;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    hidpiRes = 96f;

                return new Vector2(hdpi / hidpiRes, vdpi / hidpiRes);
            }
        }

        public override string Title
        {
            get => title;
            set
            {
                if (value != title)
                    SDL.SDL_SetWindowTitle(Pointer, title = value);
            }
        }

        public override bool Opened => !Context.Disposed;

        public override bool Bordered
        {
            get => bordered;
            set => SDL.SDL_SetWindowBordered(Pointer, (bordered = value) ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }

        public override bool Resizable
        {
            get => resizable;
            set => SDL.SDL_SetWindowResizable(Pointer, (resizable = value) ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }

        public override bool Fullscreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool Visible
        {
            get => visible;
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (!visible)
                        SDL.SDL_HideWindow(Pointer);
                    else
                        SDL.SDL_ShowWindow(Pointer);
                }
            }
        }
        public override bool VSync { get; set; } = true;

        public override bool Focused
        {
            get
            {
                var flags = (SDL.SDL_WindowFlags)SDL.SDL_GetWindowFlags(Pointer);
                return (flags & SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) == SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS;
            }
        }

        public override Vector2 Mouse
        {
            get
            {
                SDL.SDL_GetMouseState(out int x, out int y);
                return new Vector2(x, y);
            }
        }

        public override Vector2 ScreenMouse
        {
            get
            {
                SDL.SDL_GetGlobalMouseState(out int x, out int y);
                return new Vector2(x, y);
            }
        }

        public override bool MouseOver
        {
            get
            {
                var flags = (SDL.SDL_WindowFlags)SDL.SDL_GetWindowFlags(Pointer);
                return (flags & SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) == SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;
            }
        }

        internal SDL_Window(SDL_System system, string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            System = system;

            this.title = title;
            visible = !flags.HasFlag(WindowFlags.Hidden);

            var sdl = SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN;

            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                sdl |= SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            }
            else if (App.Graphics.Api == GraphicsApi.Vulkan)
            {
                sdl |= SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN;
            }

            Pointer = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, width, height, sdl);
            if (Pointer == IntPtr.Zero)
                throw new Exception($"SDL Error: {SDL.SDL_GetError()}");

            if (flags.HasFlag(WindowFlags.ScaleToMonitor))
            {
                var scale = ContentScale;
                if (scale != Vector2.One)
                {
                    var display = SDL.SDL_GetWindowDisplayIndex(Pointer);
                    SDL.SDL_GetDesktopDisplayMode(display, out var mode);
                    SDL.SDL_SetWindowPosition(Pointer, (int)(mode.w - width * scale.X) / 2, (int)(mode.h - height * scale.Y) / 2);
                    SDL.SDL_SetWindowSize(Pointer, (int)(width * scale.X), (int)(height * scale.Y));
                }
            }

            if (!flags.HasFlag(WindowFlags.Hidden))
                SDL.SDL_ShowWindow(Pointer);

            Context = new SDL_Context(system, Pointer);
        }

        public override void Close()
        {
            if (Opened)
            {
                SDL.SDL_DestroyWindow(Pointer);

                if (!Context.Disposed)
                    Context.Dispose();
            }
        }

        public override void Present()
        {
            if (lastVsync != VSync)
            {
                lastVsync = VSync;
                
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    SDL.SDL_GL_SetSwapInterval(lastVsync ? 1 : 0);
                }
            }

            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                SDL.SDL_GL_SwapWindow(Pointer);
            }
        }
    }
}
