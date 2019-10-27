using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public abstract class Shader : GraphicsResource
    {

        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;
        protected readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        protected Shader(Graphics graphics) : base(graphics)
        {
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(uniforms);
        }

    }
}
