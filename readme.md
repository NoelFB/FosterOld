![foster](icon.png)

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
 - **Platforms**: Platform implementations of the Framework modules (such as [https://www.glfw.org/](GLFW), [https://www.libsdl.org/](SDL2), OpenGL, DirectX, Vulkan, etc)
 - **Gui**: A simple dockable GUI system. Inspired by [immediate mode GUIs](https://github.com/ocornut/imgui) for widgets.
 - **Engine/Editor**: A Game Editor
 - **Engine/Runtime**: A Game Runtime

## thoughts
The Framework is meant to be completely independent and shouldn't require anything else to make a game in (much like how XNA, SFML, etc, work). The Engine & Editor are just what I've constructed to make stuff, but they are not mandatory. At some point it may make sense to separate these repos.

## dependencies
 - Each Platform implementation has its own dependencies
 - [stb_truetype](https://github.com/nothings/stb) for Font loading. We're using a [C# port](https://github.com/StbSharp/StbTrueTypeSharp)
