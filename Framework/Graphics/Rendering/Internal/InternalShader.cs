using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class InternalShader : InternalResource
    {

        protected internal readonly Dictionary<string, ShaderAttribute> attributes = new Dictionary<string, ShaderAttribute>();
        protected internal readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

    }
}
