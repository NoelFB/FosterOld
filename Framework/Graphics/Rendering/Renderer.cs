using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public enum Renderer
    {
        None = 0,
        Unknown = 1,
        OpenGLES,
        OpenGL,
        Vulkan,
        Direct3D9,
        Direct3D11,
        Direct3D12,
        Metal,
    }
}
