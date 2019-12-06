using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    // TODO:
    // Make this class way better

    public class Material
    {

        public class Parameter
        {
            public readonly ShaderUniform Uniform;

            public string Name => Uniform.Name;
            public ShaderUniform.Types Type => Uniform.Type;
            public object? Value { get; private set; }

            public Parameter(ShaderUniform uniform)
            {
                Uniform = uniform;
                Value = null;
            }

            public void SetTexture(string name, Texture? value)
            {
                if (Type == ShaderUniform.Types.Texture2D)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Sampler2D");
            }

            public void SetFloat(string name, float value)
            {
                if (Type == ShaderUniform.Types.Float)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Float");
            }

            public void SetInt(string name, int value)
            {
                if (Type == ShaderUniform.Types.Int)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Int");
            }

            public void SetMatrix(string name, Matrix2D value)
            {
                if (Type == ShaderUniform.Types.Matrix2D || Type == ShaderUniform.Types.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a 2D Matrix");
            }

            public void SetMatrix(string name, Matrix value)
            {
                if (Type == ShaderUniform.Types.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Matrix");
            }

            public void SetVector2(string name, Vector2 value)
            {
                if (Type == ShaderUniform.Types.Float2)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Vector2");
            }

            public void SetVector3(string name, Vector3 value)
            {
                if (Type == ShaderUniform.Types.Float3)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Vector3");
            }

            public void SetVector4(string name, Vector4 value)
            {
                if (Type == ShaderUniform.Types.Float4)
                    Value = value;
                else
                    throw new Exception($"Parameter {name} isn't a Vector4");
            }

            public void SetColor(string name, Color value)
            {
                if (Type == ShaderUniform.Types.Float4)
                    Value = value.ToVector4();
                else
                    throw new Exception($"Parameter {name} isn't a Vector4");
            }
        }

        public readonly Shader Shader;

        public readonly Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

        public Material(Shader shader)
        {
            Shader = shader;

            foreach (var uniform in shader.Uniforms.Values)
                Parameters.Add(uniform.Name, new Parameter(uniform));
        }

        public void SetTexture(string name, Texture? value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetTexture(name, value);
        }

        public void SetFloat(string name, float value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetFloat(name, value);
        }

        public void SetInt(string name, int value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetInt(name, value);
        }

        public void SetMatrix(string name, Matrix2D value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetMatrix(name, value);
        }

        public void SetMatrix(string name, Matrix value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetMatrix(name, value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetVector2(name, value);
        }

        public void SetVector3(string name, Vector3 value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetVector3(name, value);
        }

        public void SetVector4(string name, Vector4 value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetVector4(name, value);
        }

        public void SetColor(string name, Color value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.SetColor(name, value);
        }

    }
}
