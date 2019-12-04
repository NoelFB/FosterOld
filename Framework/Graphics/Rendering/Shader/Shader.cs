using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class Shader : GraphicsResource
    {

        public readonly ReadOnlyDictionary<string, ShaderAttribute> Attributes;
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;

        protected readonly Dictionary<string, ShaderAttribute> attributes = new Dictionary<string, ShaderAttribute>();
        protected readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        protected Shader(Graphics graphics) : base(graphics)
        {
            Attributes = new ReadOnlyDictionary<string, ShaderAttribute>(attributes);
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(uniforms);
        }

    }
}
