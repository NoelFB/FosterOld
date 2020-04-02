using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

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
            /// Array Length (1 for single values)
            /// </summary>
            public int Length => Uniform.Length;

            /// <summary>
            /// Whether the Parameter is an Array
            /// </summary>
            public bool IsArray => Length > 1;

            /// <summary>
            /// The Value of the Parameter
            /// </summary>
            public object Value { get; private set; }

            public Parameter(ShaderUniform uniform)
            {
                Uniform = uniform;

                Value = uniform.Type switch
                {
                    UniformType.Int => new int[uniform.Length],
                    UniformType.Float => new float[uniform.Length],
                    UniformType.Float2 => new float[uniform.Length * 2],
                    UniformType.Float3 => new float[uniform.Length * 3],
                    UniformType.Float4 => new float[uniform.Length * 4],
                    UniformType.Matrix3x2 => new float[uniform.Length * 6],
                    UniformType.Matrix4x4 => new float[uniform.Length * 16],
                    UniformType.Sampler => new Texture?[uniform.Length],
                    UniformType.Unknown => null!,
                    _ => null!
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void AssertParameters(UniformType expected, int index)
            {
                if (Type != expected)
                    throw new Exception($"Parameter {Name} isn't a {expected}");

                if (index < 0 || index > Length)
                    throw new IndexOutOfRangeException($"Paramater {Name} Index is out range");
            }

            public void SetTexture(Texture? value) => SetTexture(0, value);
            public void SetTexture(int index, Texture? value)
            {
                AssertParameters(UniformType.Sampler, index);

                if (Value is Texture?[] textures)
                    textures[index] = value;
            }

            public Texture? GetTexture(int index = 0)
            {
                AssertParameters(UniformType.Sampler, index);

                if (Value is Texture?[] textures)
                    return textures[index];

                return null;
            }

            public void SetInt(int value) => SetInt(0, value);
            public void SetInt(int index, int value)
            {
                AssertParameters(UniformType.Int, index);

                if (Value is int[] values)
                    values[index] = value;
            }

            public int GetInt(int index = 0)
            {
                AssertParameters(UniformType.Int, index);

                if (Value is int[] values)
                    return values[index];

                return 0;
            }

            public void SetFloat(float value) => SetFloat(0, value);
            public void SetFloat(int index, float value)
            {
                AssertParameters(UniformType.Float, index);

                if (Value is float[] values)
                    values[index] = value;
            }

            public float GetFloat(int index = 0)
            {
                AssertParameters(UniformType.Float, index);

                if (Value is float[] values)
                    return values[index];

                return 0;
            }

            public void SetVector2(in Vector2 value) => SetVector2(0, value);
            public void SetVector2(int index, in Vector2 value)
            {
                AssertParameters(UniformType.Float2, index);

                var offset = index * 2;
                if (Value is float[] values)
                {
                    values[offset + 0] = value.X;
                    values[offset + 1] = value.Y;
                }
            }

            public Vector2 GetVector2(int index = 0)
            {
                AssertParameters(UniformType.Float2, index);

                if (Value is float[] values)
                {
                    var offset = index * 2;
                    return new Vector2(values[offset + 0], values[offset + 1]);
                }

                return Vector2.Zero;
            }

            public void SetVector3(in Vector3 value) => SetVector3(0, value);

            public void SetVector3(int index, in Vector3 value)
            {
                AssertParameters(UniformType.Float3, index);

                var offset = index * 3;
                if (Value is float[] values)
                {
                    values[offset + 0] = value.X;
                    values[offset + 1] = value.Y;
                    values[offset + 2] = value.Z;
                }
            }

            public Vector3 GetVector3(int index = 0)
            {
                AssertParameters(UniformType.Float3, index);

                if (Value is float[] values)
                {
                    var offset = index * 3;
                    return new Vector3(values[offset + 0], values[offset + 1], values[offset + 2]);
                }

                return Vector3.Zero;
            }

            public void SetVector4(in Vector4 value) => SetVector4(0, value);

            public void SetVector4(int index, in Vector4 value)
            {
                AssertParameters(UniformType.Float4, index);

                var offset = index * 4;
                if (Value is float[] values)
                {
                    values[offset + 0] = value.X;
                    values[offset + 1] = value.Y;
                    values[offset + 2] = value.Z;
                    values[offset + 3] = value.W;
                }
            }

            public Vector4 GetVector4(int index = 0)
            {
                AssertParameters(UniformType.Float4, index);

                if (Value is float[] values)
                {
                    var offset = index * 4;
                    return new Vector4(values[offset + 0], values[offset + 1], values[offset + 2], values[offset + 3]);
                }

                return Vector4.Zero;
            }

            public void SetMatrix3x2(in Matrix3x2 value) => SetMatrix3x2(0, value);

            public void SetMatrix3x2(int index, in Matrix3x2 value)
            {
                AssertParameters(UniformType.Matrix3x2, index);

                var offset = index * 6;
                if (Value is float[] values)
                {
                    values[offset + 0] = value.M11;
                    values[offset + 1] = value.M12;
                    values[offset + 2] = value.M21;
                    values[offset + 3] = value.M22;
                    values[offset + 4] = value.M31;
                    values[offset + 5] = value.M32;
                }
            }

            public Matrix3x2 GetMatrix3x2(int index = 0)
            {
                AssertParameters(UniformType.Float4, index);

                if (Value is float[] values)
                {
                    var offset = index * 6;
                    return new Matrix3x2(
                        values[offset + 0],
                        values[offset + 1],
                        values[offset + 2],
                        values[offset + 3],
                        values[offset + 4],
                        values[offset + 5]
                        );
                }

                return Matrix3x2.Identity;
            }

            public void SetMatrix4x4(in Matrix4x4 value) => SetMatrix4x4(0, value);

            public void SetMatrix4x4(int index, in Matrix4x4 value)
            {
                AssertParameters(UniformType.Matrix4x4, index);

                var offset = index * 16;
                if (Value is float[] values)
                {
                    values[offset + 00] = value.M11;
                    values[offset + 01] = value.M12;
                    values[offset + 02] = value.M13;
                    values[offset + 03] = value.M14;
                    values[offset + 04] = value.M21;
                    values[offset + 05] = value.M22;
                    values[offset + 06] = value.M23;
                    values[offset + 07] = value.M24;
                    values[offset + 08] = value.M31;
                    values[offset + 09] = value.M32;
                    values[offset + 10] = value.M33;
                    values[offset + 11] = value.M34;
                    values[offset + 12] = value.M41;
                    values[offset + 13] = value.M42;
                    values[offset + 14] = value.M43;
                    values[offset + 15] = value.M44;
                }

            }

            public Matrix4x4 GetMatrix4x4(int index = 0)
            {
                AssertParameters(UniformType.Float4, index);

                if (Value is float[] values)
                {
                    var offset = index * 16;
                    return new Matrix4x4(
                        values[offset + 00],
                        values[offset + 01],
                        values[offset + 02],
                        values[offset + 03],
                        values[offset + 04],
                        values[offset + 05],
                        values[offset + 06],
                        values[offset + 07],
                        values[offset + 08],
                        values[offset + 09],
                        values[offset + 10],
                        values[offset + 11],
                        values[offset + 12],
                        values[offset + 13],
                        values[offset + 14],
                        values[offset + 15]
                        );
                }

                return Matrix4x4.Identity;
            }

            public void SetColor(in Color value) => SetColor(0, value);

            public void SetColor(int index, in Color value)
            {
                AssertParameters(UniformType.Float4, index);
                var vec = value.ToVector4();
                var offset = index * 4;
                if (Value is float[] values)
                {
                    values[offset + 0] = vec.X;
                    values[offset + 1] = vec.Y;
                    values[offset + 2] = vec.Z;
                    values[offset + 3] = vec.W;
                }
            }

            public Color GetColor(int index = 0)
            {
                AssertParameters(UniformType.Float4, index);

                if (Value is float[] values)
                {
                    var offset = index * 4;
                    return new Color(values[offset + 0], values[offset + 1], values[offset + 2], values[offset + 3]);
                }

                return Color.Transparent;
            }
        }

        /// <summary>
        /// The Shader this Material uses
        /// </summary>
        public readonly Shader Shader;

        /// <summary>
        /// The list of all Parameters within this Material
        /// </summary>
        public readonly ReadOnlyCollection<Parameter> Parameters;

        private readonly Dictionary<string, Parameter> parametersByName = new Dictionary<string, Parameter>();

        public Material(Shader shader)
        {
            Shader = shader;

            var parameters = new List<Parameter>();

            foreach (var uniform in shader.Uniforms.Values)
            {
                var parameter = new Parameter(uniform);
                parametersByName[uniform.Name] = parameter;
                parameters.Add(parameter);
            }

            Parameters = new ReadOnlyCollection<Parameter>(parameters);
        }

        /// <summary>
        /// Gets the Parameter with the given name and returns true if found
        /// </summary>
        public bool TryGetParameter(string name, [MaybeNullWhen(false)] out Parameter parameter)
        {
            if (parametersByName.TryGetValue(name, out parameter!))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the Parameter with the given name
        /// </summary>
        public Parameter this[string name] => parametersByName[name];

    }
}
