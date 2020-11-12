# Foster
Foster is an open source & cross-platform game framework made in C# dotnet core.

_★ very work in progress! likely to have frequent, breaking changes! please use at your own risk! ★_

## goals
 - Designed for small desktop + console 2D and 3D games; No mobile
 - A simple and lightweight Platform API so it's easy to swap out the guts
 - As few dependencies outside of C# as possible
 - Modern C# without worry about backwards compatibility
 - This isn't built for modern graphics or like a billion polygons
 - This isn't meant to have Game Engine-specific code, but rather just contain Windowing, Rendering, Input, and Audio

## what's here
 - **Framework**: The main Framework with an abstract Platform implementation. Handles Input, Drawing, Windowing, etc.
 - **Platforms**: Platform implementations of the Framework modules (such as [GLFW](https://www.glfw.org/), [SDL2](https://www.libsdl.org/), OpenGL, DirectX, Vulkan, etc)
 - **JSON**: Simple JSON reading / writing library. Can also read and write non-strict JSON (like [Hjson](https://hjson.github.io/))

## dependencies
 - [dotnet core 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) and [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
 - [stb_truetype](https://github.com/nothings/stb) for Font loading. We're using a [C# port](https://github.com/StbSharp/StbTrueTypeSharp)
 - Each Platform implementation has its own dependencies

## getting started
Check out the [Wiki](https://github.com/NoelFB/Foster/wiki) for Guides and API references

## inspiration
Taken a lot of inspiration from other Frameworks and APIs, namely [FNA](https://fna-xna.github.io/) and [Halley Game Engine](https://github.com/amzeratul/halley)
