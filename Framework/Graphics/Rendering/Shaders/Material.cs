using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace Foster.Framework
{

    public class Material
    {

        public class Parameter
        {
            /// <summary>
            /// Reference to the Parameter's Uniform
            /// </summary>
            public readonly ShaderUniform Uniform;

            /// <summary>
            /// The Name of the Parameter
            /// </summary>
            public string Name => Uniform.Name;

            /// <summary>
            /// The Shader Uniform Type of the Parameter
            /// </summary>
            public ShaderUniform.Types Type => Uniform.Type;

            /// <summary>
            /// The Value of the Parameter
            /// </summary>
            public object? Value { get; private set; }

            public Parameter(ShaderUniform uniform)
            {
                Uniform = uniform;
                Value = null;
            }

            public void SetTexture(Texture? value)
            {
                if (Type == ShaderUniform.Types.Texture2D)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Sampler2D");
            }

            public void SetFloat(float value)
            {
                if (Type == ShaderUniform.Types.Float)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Float");
            }

            public void SetInt(int value)
            {
                if (Type == ShaderUniform.Types.Int)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Int");
            }

            public void SetMatrix(Matrix2D value)
            {
                if (Type == ShaderUniform.Types.Matrix2D || Type == ShaderUniform.Types.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a 2D Matrix");
            }

            public void SetMatrix(Matrix value)
            {
                if (Type == ShaderUniform.Types.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Matrix");
            }

            public void SetVector2(Vector2 value)
            {
                if (Type == ShaderUniform.Types.Float2)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector2");
            }

            public void SetVector3(Vector3 value)
            {
                if (Type == ShaderUniform.Types.Float3)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector3");
            }

            public void SetVector4(Vector4 value)
            {
                if (Type == ShaderUniform.Types.Float4)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector4");
            }

            public void SetColor(Color value)
            {
                if (Type == ShaderUniform.Types.Float4)
                    Value = value.ToVector4();
                else
                    throw new Exception($"Parameter {Name} isn't a Vector4");
            }
        }

        public readonly Shader Shader;
        public readonly ReadOnlyDictionary<string, Parameter> Parameters;

        public Material(Shader shader)
        {
            Shader = shader;

            var parameters = new Dictionary<string, Parameter>();
            foreach (var uniform in shader.Uniforms.Values)
                parameters.Add(uniform.Name, new Parameter(uniform));
            Parameters = new ReadOnlyDictionary<string, Parameter>(parameters);
        }

        public Parameter? this[string name]
        {
            get
            {
                if (Parameters.TryGetValue(name, out var parameter))
                    return parameter;
                return null;
            }
        }

    }
}
