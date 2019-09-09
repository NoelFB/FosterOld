using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public enum UniformType
    {
        Int = 0x100,

        Float = 0x200,
        Float2,
        Float3,
        Float4,

        Matrix3x2 = 0x300,
        Matrix4x4,

        Sampler2D = 0x400
    }
}
