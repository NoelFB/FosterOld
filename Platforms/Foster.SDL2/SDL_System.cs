using Foster.Framework;
using SDL2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Xsl;

namespace Foster.SDL2
{
    /// <summary>
    /// TODO:
    /// The SDL2 implementation is NOT done
    /// </summary>
    public class SDL_System : Framework.System
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDPIAware();

        private IntPtr hiddenWindow;

        public override bool SupportsMultipleWindows => true;

        internal List<Window> windows = new List<Window>();
        internal List<Framework.Monitor> monitors = new List<Framework.Monitor>();
        internal List<Context> contexts = new List<Context>();

        public override ReadOnlyCollection<Window> Windows { get; }
        public override ReadOnlyCollection<Framework.Monitor> Monitors { get; }
        protected override ReadOnlyCollection<Context> Contexts { get; }

        public SDL_System()
        {
            Windows = new ReadOnlyCollection<Window>(windows);
            Monitors = new ReadOnlyCollection<Framework.Monitor>(monitors);
            Contexts = new ReadOnlyCollection<Context>(contexts);
        }

        protected override void Initialized()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetProcessDPIAware();

            // get API info
            {
                SDL.SDL_GetVersion(out var version);
                ApiName = "SDL2";
                ApiVersion = new Version(version.major, version.minor, version.patch);
            }

            base.Initialized();
        }

        protected override void Startup()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_TIMER | SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_JOYSTICK | SDL.SDL_INIT_GAMECONTROLLER) != 0)
            {
                SDL.SDL_Quit();
                throw new Exception($"SDL Error: {SDL.SDL_GetError()}");
            }

            // GLFW only supports OpenGL and Vulkan
            if (App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
                throw new Exception("SDL Only supports OpenGL and Vulkan Graphics APIs");

            // macOS requires versions to be set to 3.2
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
                    SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
                    SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, 0x00032001);
                }
            }

            // various attributes
            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
                SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_SHARE_WITH_CURRENT_CONTEXT, 1);
            }

            SDL.SDL_CaptureMouse(SDL.SDL_bool.SDL_TRUE);

            // Monitors
            /*
            unsafe
            {
                var monitorPtrs = GLFW.GetMonitors(out int count);
                for (int i = 0; i < count; i++)
                    monitors.Add(new GLFW_Monitor(monitorPtrs[i]));
            }
            */

            // create a hidden window so we can have a default context
            {
                var flags = SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN;
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    flags |= SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
                }
                
                hiddenWindow = SDL.SDL_CreateWindow("hidden-context", 0, 0, 128, 128, flags);
                if (hiddenWindow == IntPtr.Zero)
                    throw new Exception($"SDL Error: {SDL.SDL_GetError()}");
            }

            // Our default shared context
            CreateContext();
            SetCurrentContext(Contexts[0]);

            base.Startup();
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            SDL.SDL_Quit();
        }

        protected override void AfterUpdate()
        {
            while (SDL.SDL_PollEvent(out var ev) != 0)
            {

            }

            // Remove closed windows
            for (int i = windows.Count - 1; i >= 0; i--)
                if (!windows[i].Opened)
                    windows.RemoveAt(i);
        }

        public override Context CreateContext()
        {
            return new SDL_Context(this, hiddenWindow);
        }

        public override Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            var window = new SDL_Window(this, title, width, height, flags);
            windows.Add(window);
            return window;
        }

        public override IntPtr GetProcAddress(string name)
        {
            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                return SDL.SDL_GL_GetProcAddress(name);
            }

            throw new NotImplementedException();
        }

        protected override void SetCurrentContextInternal(Context? context)
        {
            if (App.Graphics.Api == GraphicsApi.OpenGL)
            {
                if (context is SDL_Context ctx)
                    SDL.SDL_GL_MakeCurrent(ctx.Window, ctx.Pointer);
            }
        }
    }

}
