using Foster.Framework;
using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.SDL2
{
    internal class SDL_Window : Window.Platform
    {
        public readonly IntPtr SDLWindowPtr;
        public readonly uint SDLWindowID;

        internal readonly SDL_System system;
        internal readonly SDL_GLContext? glContext;

        private bool isBordered = true;
        private bool isFullscreen = false;
        private bool isResizable = false;
        private bool isVisible = false;
        private bool isVSyncEnabled = true;
        private bool isClosed = false;

        protected override IntPtr Pointer
        {
            get
            {
                var info = new SDL.SDL_SysWMinfo();
                SDL.SDL_VERSION(out info.version);
                SDL.SDL_GetWindowWMInfo(SDLWindowPtr, ref info);

                switch (info.subsystem)
                {
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS:
                        return info.info.win.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_X11:
                        return info.info.x11.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_DIRECTFB:
                        return info.info.dfb.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_COCOA:
                        return info.info.cocoa.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_UIKIT:
                        return info.info.uikit.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_WAYLAND:
                        return info.info.wl.surface;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_MIR:
                        return info.info.mir.surface;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_WINRT:
                        return info.info.winrt.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_ANDROID:
                        return info.info.android.window;
                    case SDL.SDL_SYSWM_TYPE.SDL_SYSWM_UNKNOWN:
                        break;
                }

                throw new NotImplementedException();
            }
        }

        protected override Point2 Position
        {
            get
            {
                SDL.SDL_GetWindowPosition(SDLWindowPtr, out int x, out int y);
                return new Point2(x, y);
            }
            set
            {
                SDL.SDL_SetWindowPosition(SDLWindowPtr, value.X, value.Y);
            }
        }

        protected override Point2 Size
        {
            get
            {
                SDL.SDL_GetWindowSize(SDLWindowPtr, out int w, out int h);
                return new Point2(w, h);
            }
            set
            {
                SDL.SDL_SetWindowSize(SDLWindowPtr, value.X, value.Y);
            }
        }

        protected override Point2 RenderSize
        {
            get
            {
                int w, h;

                if (App.Graphics is IGraphicsOpenGL)
                    SDL.SDL_GL_GetDrawableSize(SDLWindowPtr, out w, out h);
                else if (App.Graphics is IGraphicsVulkan)
                    SDL.SDL_Vulkan_GetDrawableSize(SDLWindowPtr, out w, out h);
                else
                    SDL.SDL_GetWindowSize(SDLWindowPtr, out w, out h);

                return new Point2(w, h);
            }
        }

        protected override Vector2 ContentScale
        {
            get
            {
                float hidpiRes = 72f;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    hidpiRes = 96;

                var index = SDL.SDL_GetWindowDisplayIndex(SDLWindowPtr);
                SDL.SDL_GetDisplayDPI(index, out float ddpi, out _, out _);
                return Vector2.One * (ddpi / hidpiRes);
            }
        }

        protected override bool Opened => !isClosed;

        protected override string Title
        {
            get => SDL.SDL_GetWindowTitle(SDLWindowPtr);
            set => SDL.SDL_SetWindowTitle(SDLWindowPtr, value);
        }

        protected override bool Bordered
        {
            get => isBordered;
            set
            {
                if (isBordered != value)
                {
                    isBordered = value;
                    SDL.SDL_SetWindowBordered(SDLWindowPtr, isBordered ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
                }
            }
        }

        protected override bool Resizable
        {
            get => isResizable;
            set
            {
                if (isResizable != value)
                {
                    isResizable = value;
                    SDL.SDL_SetWindowResizable(SDLWindowPtr, isResizable ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
                }
            }
        }

        protected override bool Fullscreen
        {
            get => isFullscreen;
            set
            {
                if (isFullscreen != value)
                {
                    isFullscreen = value;
                    if (isFullscreen)
                        SDL.SDL_SetWindowFullscreen(SDLWindowPtr, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
                    else
                        SDL.SDL_SetWindowFullscreen(SDLWindowPtr, (uint)0);
                }
            }
        }

        protected override bool Visible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    if (isVisible)
                        SDL.SDL_ShowWindow(SDLWindowPtr);
                    else
                        SDL.SDL_HideWindow(SDLWindowPtr);
                }
            }
        }

        protected override bool VSync
        {
            get => isVSyncEnabled;
            set => isVSyncEnabled = value;
        }

        protected override bool Focused
        {
            get => (SDL.SDL_GetWindowFlags(SDLWindowPtr) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) > 0;
        }

        protected override Vector2 Mouse
        {
            get
            {
                SDL.SDL_GetWindowPosition(SDLWindowPtr, out int winX, out int winY);
                SDL.SDL_GetGlobalMouseState(out int x, out int y);
                return new Vector2(x - winX, y - winY);
            }
        }

        protected override Vector2 ScreenMouse
        {
            get
            {
                SDL.SDL_GetGlobalMouseState(out int x, out int y);
                return new Vector2(x, y);
            }
        }

        protected override bool MouseOver
        {
            get => (SDL.SDL_GetWindowFlags(SDLWindowPtr) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) > 0;
        }

        public SDL_Window(SDL_System system, string title, int width, int height, WindowFlags flags)
        {
            this.system = system;

            var sdlWindowFlags = 
                SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | 
                SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN | 
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            if (flags.HasFlag(WindowFlags.Fullscreen))
            {
                sdlWindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;
                isFullscreen = true;
            }

            if (App.Graphics is IGraphicsOpenGL)
            {
                sdlWindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;

                if (system.Windows.Count > 0)
                    SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_SHARE_WITH_CURRENT_CONTEXT, 1);
            }

            // create the window
            SDLWindowPtr = SDL.SDL_CreateWindow(title, 0x2FFF0000, 0x2FFF0000, width, height, sdlWindowFlags);
            if (SDLWindowPtr == IntPtr.Zero)
                throw new Exception($"Failed to create a new Window: {SDL.SDL_GetError()}");
            SDLWindowID = SDL.SDL_GetWindowID(SDLWindowPtr);

            if (flags.HasFlag(WindowFlags.Transparent))
                SDL.SDL_SetWindowOpacity(SDLWindowPtr, 0f);

            // scale to monitor for HiDPI displays
            if (flags.HasFlag(WindowFlags.ScaleToMonitor))
            {
                float hidpiRes = 72f;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    hidpiRes = 96;

                var display = SDL.SDL_GetWindowDisplayIndex(SDLWindowPtr);
                SDL.SDL_GetDisplayDPI(display, out var ddpi, out var hdpi, out var vdpi);

                var dpi = (ddpi / hidpiRes);
                if (dpi != 1)
                {
                    SDL.SDL_GetDesktopDisplayMode(display, out var mode);
                    SDL.SDL_SetWindowPosition(SDLWindowPtr, (int)(mode.w - width * dpi) / 2, (int)(mode.h - height * dpi) / 2);
                    SDL.SDL_SetWindowSize(SDLWindowPtr, (int)(width * dpi), (int)(height * dpi));
                }
            }

            // create the OpenGL context
            if (App.Graphics is IGraphicsOpenGL)
            {
                glContext = new SDL_GLContext(system, SDLWindowPtr);
                system.SetCurrentGLContext(glContext);
            }

            // show window
            isVisible = false;
            if (!flags.HasFlag(WindowFlags.Hidden))
            {
                isVisible = true;
                SDL.SDL_ShowWindow(SDLWindowPtr);
            }
        }

        protected override void Focus()
        {
            SDL.SDL_RaiseWindow(SDLWindowPtr);
        }

        protected override void Present()
        {
            if (App.Graphics is IGraphicsOpenGL)
            {
                system.SetCurrentGLContext(glContext);
                SDL.SDL_GL_SetSwapInterval(isVSyncEnabled ? 1 : 0);
                SDL.SDL_GL_SwapWindow(SDLWindowPtr);
            }
        }

        protected override void Close()
        {
            if (!isClosed)
            {
                isClosed = true;

                OnClose?.Invoke();
                glContext?.Dispose();
                SDL.SDL_DestroyWindow(SDLWindowPtr);
            }
        }

        public void Resized()
        {
            OnResize?.Invoke();
        }

        public void CloseRequested()
        {
            OnCloseRequested?.Invoke();
        }

        public void FocusGained()
        {
            OnFocus?.Invoke();
        }

        public void Minimized()
        {
            isVisible = false;
        }

        public void Restored()
        {
            isVisible = true;
        }
    }
}
