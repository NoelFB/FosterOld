using System.Collections.Generic;

namespace Foster.Framework
{
    public class Material
    {

        public class Parameter
        {
            public readonly ShaderUniform Uniform;

            public string Name => Uniform.Name;
            public UniformType Type => Uniform.Type;
            public object? Value;

            public Parameter(ShaderUniform uniform)
            {
                Uniform = uniform;
                Value = null;
            }
        }

        private Shader? shader;

        public Shader? Shader
        {
            get => shader;
            set
            {
                if (shader != value)
                {
                    shader = value;

                    Textures.Clear();
                    Parameters.Clear();

                    if (shader != null)
                    {
                        foreach (ShaderUniform sampler in shader.Textures)
                        {
                            Textures.Add(new Parameter(sampler));
                        }

                        foreach (ShaderUniform uniform in shader.Uniforms.Values)
                        {
                            Parameters.Add(uniform.Name, new Parameter(uniform));
                        }
                    }
                }
            }
        }

        public readonly List<Parameter> Textures = new List<Parameter>();
        public readonly Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

        public Material(Shader? shader)
        {
            Shader = shader;
        }

        public void SetTexture(int slot, Texture? texture)
        {
            if (Shader != null && Textures.Count > slot)
            {
                Textures[slot].Value = texture;
            }
        }

        public void Apply()
        {
            if (Shader != null)
            {
                foreach (Parameter param in Parameters.Values)
                {
                    param.Uniform.Value = param.Value;
                }
            }
        }

    }
}
