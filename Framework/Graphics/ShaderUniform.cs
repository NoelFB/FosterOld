using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class ShaderUniform
    {
        public string Name { get; protected set; } = "";
        public UniformType Type { get; protected set; } = UniformType.Unknown;
        public abstract object? Value { get; set; }
    }
}
