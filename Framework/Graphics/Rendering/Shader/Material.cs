using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace Foster.Framework
{
    /// <summary>
    /// A Material is used to store the state of a Shader
    /// </summary>
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
            public UniformType Type => Uniform.Type;

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
                if (Type == UniformType.Texture2D)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Sampler2D");
            }

            public void SetFloat(float value)
            {
                if (Type == UniformType.Float)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Float");
            }

            public void SetInt(int value)
            {
                if (Type == UniformType.Int)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Int");
            }

            public void SetMatrix(Matrix2D value)
            {
                if (Type == UniformType.Matrix2D || Type == UniformType.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a 2D Matrix");
            }

            public void SetMatrix(Matrix value)
            {
                if (Type == UniformType.Matrix)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Matrix");
            }

            public void SetVector2(Vector2 value)
            {
                if (Type == UniformType.Float2)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector2");
            }

            public void SetVector3(Vector3 value)
            {
                if (Type == UniformType.Float3)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector3");
            }

            public void SetVector4(Vector4 value)
            {
                if (Type == UniformType.Float4)
                    Value = value;
                else
                    throw new Exception($"Parameter {Name} isn't a Vector4");
            }

            public void SetColor(Color value)
            {
                if (Type == UniformType.Float4)
                    Value = value.ToVector4();
                else
                    throw new Exception($"Parameter {Name} isn't a Vector4");
            }
        }

        /// <summary>
        /// The Shader this Material uses
        /// </summary>
        public readonly Shader Shader;

        /// <summary>
        /// The list of all Parameters within this Material, based on its shader
        /// </summary>
        public readonly ReadOnlyDictionary<string, Parameter> Parameters;

        public Material(Shader shader)
        {
            Shader = shader;

            var parameters = new Dictionary<string, Parameter>();
            foreach (var uniform in shader.Uniforms.Values)
                parameters.Add(uniform.Name, new Parameter(uniform));
            Parameters = new ReadOnlyDictionary<string, Parameter>(parameters);
        }

        /// <summary>
        /// Gets the Parameter with the given name, or null if it doesn't exist
        /// </summary>
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
