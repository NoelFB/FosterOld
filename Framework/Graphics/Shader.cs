using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public abstract class Shader : GraphicsResource
    {
        public readonly ReadOnlyCollection<ShaderUniform> Textures;
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;

        protected readonly List<ShaderUniform> textures = new List<ShaderUniform>();
        protected readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        protected Shader(Graphics graphics) : base(graphics)
        {
            Textures = textures.AsReadOnly();
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(uniforms);
        }

        public void SetUniform(string name, float value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Float)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Float");
        }

        public void SetUniform(string name, int value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Int)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Int");
        }

        public void SetUniform(string name, Matrix3x2 value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && (uniform.Type == UniformType.Matrix3x2 || uniform.Type == UniformType.Matrix4x4))
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Matrix");
        }

        public void SetUniform(string name, Matrix4x4 value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Matrix4x4)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Matrix4x4");
        }

        public void SetUniform(string name, Texture? value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Texture2D)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Sampler2D");
        }

        public void SetUniform(string name, Vector2 value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Float2)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Vector2");
        }

        public void SetUniform(string name, Vector3 value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Float3)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Vector3");
        }

        public void SetUniform(string name, Vector4 value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Float4)
                uniform.Value = value;
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Vector4");
        }

        public void SetUniform(string name, Color value)
        {
            if (uniforms.TryGetValue(name, out var uniform) && uniform.Type == UniformType.Float4)
                uniform.Value = value.ToVector4();
            else
                throw new Exception($"Uniform {name} doesn't exist or isn't a Vector4");
        }

        public abstract void Use();
    }
}
