# foster
foster is an open source & cross-platform game framework made in C# dotnet core.

## goals
 - Easy to use for small games
 - Designed for desktop + console 2D and 3D games; no mobile
 - As few dependencies outside of C# as possible
 - Modern C# - don't care about backwards compatibility
 - A simple and lightweight platform API so it's easy to swap out the guts
 - Something to iterate on. I just enjoy making this stuff and it's not trying to be the best possible framework ever
 - This isn't built for modern graphics or like a billion polygons.

## what's here
 - **Framework**: The main Framework with an abstract Platform implementation. Handles Input, Drawing, Windowing, etc.
 - **Platforms**: Platform implementations of the Framework modules (such as [GLFW](https://www.glfw.org/), [SLD2](https://www.libsdl.org/), OpenGL, DirectX, Vulkan, etc)
 - **Gui**: A simple dockable GUI system. Inspired by [immediate mode GUIs](https://github.com/ocornut/imgui) for widgets.
 - **Engine/Editor**: A Game Editor
 - **Engine/Runtime**: A Game Runtime

## thoughts
The Framework is meant to be completely independent and shouldn't require anything else to make a game in (much like how XNA, SFML, etc, work). The Engine & Editor are just what I've constructed to make stuff, but they are not mandatory. At some point it may make sense to separate these repos.

## dependencies
 - [dotnet core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) and [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
 - [stb_truetype](https://github.com/nothings/stb) for Font loading. We're using a [C# port](https://github.com/StbSharp/StbTrueTypeSharp)
 - Each Platform implementation has its own dependencies

## example app
```
using Foster.Framework;

internal class Program
{

    private static void Main(string[] args)
    {
        // register core modules (system, input, graphics)
        App.Modules.Register<GLFW.GLFW_System>();
        App.Modules.Register<GLFW.GLFW_Input>();
        App.Modules.Register<OpenGL.GL_Graphics>();

        // register our custom game module
        App.Modules.Register<Game>();

        // start the application
        App.Start();
    }

    private class Game : Module
    {
        private Batch2D batch;
        private Window window;

        protected override void Startup()
        {
            window = App.System.CreateWindow("my app", 1280, 720);
            window.OnRender += Render;
            window.OnClose += App.Exit;

            batch = new Batch2D();
        }

        protected override void Update()
        {
            batch.Clear();
            batch.Rect(32, 32, 64, 64, Color.Red);
        }

        private void Render(Window window)
        {
            App.Graphics.Clear(window, Color.Black);
            batch.Render(window);
        }
    }
}
```
