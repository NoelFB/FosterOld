using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public abstract class Shader : GraphicsResource
    {

        public readonly ReadOnlyDictionary<string, Uniform> Uniforms;
        protected readonly Dictionary<string, Uniform> uniforms = new Dictionary<string, Uniform>();

        protected Shader(Graphics graphics) : base(graphics)
        {
            Uniforms = new ReadOnlyDictionary<string, Uniform>(uniforms);
        }

    }
}
