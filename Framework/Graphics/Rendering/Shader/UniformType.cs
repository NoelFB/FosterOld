using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A Shader Uniform Type
    /// </summary>
    public enum UniformType
    {
        Unknown,
        Int,
        Float,
        Float2,
        Float3,
        Float4,
        Matrix2D,
        Matrix,
        Sampler
    }
}
