# Foster
Foster is an open source & cross-platform game framework made in C# dotnet core.

_★ very work in progress! likely to have frequent, breaking changes ★_

## goals
 - Designed for small desktop + console 2D and 3D games; No mobile
 - A simple and lightweight Platform API so it's easy to swap out the guts
 - As few dependencies outside of C# as possible
 - Modern C# without worry about backwards compatibility
 - This isn't built for modern graphics or like a billion polygons
 - This isn't meant to have Game or Engine specific code, but rather just contain Windowing, Rendering, Input, and Audio

## what's here
 - **Framework**: The main Framework with an abstract Platform implementation. Handles Input, Drawing, Windowing, etc.
 - **Platforms**: Platform implementations of the Framework modules (such as [GLFW](https://www.glfw.org/), [SLD2](https://www.libsdl.org/), OpenGL, DirectX, Vulkan, etc)
 - **JSON**: Simple JSON reading / writing library. Can also read and write non-strict JSON (like [Hjson](https://hjson.github.io/))

## dependencies
 - [dotnet core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) and [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
 - [stb_truetype](https://github.com/nothings/stb) for Font loading. We're using a [C# port](https://github.com/StbSharp/StbTrueTypeSharp)
 - Each Platform implementation has its own dependencies

## example app
```cs
using Foster.Framework;

internal class Program
{
    private static void Main(string[] args)
    {
        // register core modules (system, graphics)
        App.Modules.Register<Foster.GLFW.GLFW_System>();
        App.Modules.Register<Foster.OpenGL.GL_Graphics>();

        // register our custom game module
        App.Modules.Register<Game>();

        // start the application
        App.Start("Game", 1280, 720, WindowFlags.ScaleToMonitor);
    }

    private class Game : Module
    {
        private readonly Batch2D batch = new Batch2D();

        protected override void Startup()
        {
            App.Window.OnRender += Render;
        }

        protected override void Shutdown()
        {
            App.Window.OnRender -= Render;
        }

        private void Render(Window window)
        {
            App.Graphics.Clear(window, Color.Black);

            batch.Clear();
            batch.Rect(32, 32, 64, 64, Color.Red);
            batch.Render(window);
        }
    }
}
```

## inspiration
Taken a lot of inspiration from other Frameworks and APIs, namely [FNA](https://fna-xna.github.io/) and [Halley Game Engine](https://github.com/amzeratul/halley)
