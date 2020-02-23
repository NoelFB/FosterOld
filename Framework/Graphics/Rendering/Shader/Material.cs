using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            public Array Value { get; private set; }

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
                Value.SetValue(value, index);
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
                Value.SetValue(value, index);
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
                Value.SetValue(value, index);
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
                Value.SetValue(value.X, offset + 0);
                Value.SetValue(value.Y, offset + 1);
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
                Value.SetValue(value.X, offset + 0);
                Value.SetValue(value.Y, offset + 1);
                Value.SetValue(value.Z, offset + 2);
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
                Value.SetValue(value.X, offset + 0);
                Value.SetValue(value.Y, offset + 1);
                Value.SetValue(value.Z, offset + 2);
                Value.SetValue(value.W, offset + 3);
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
                Value.SetValue(value.M11, offset + 0);
                Value.SetValue(value.M12, offset + 1);
                Value.SetValue(value.M21, offset + 2);
                Value.SetValue(value.M22, offset + 3);
                Value.SetValue(value.M31, offset + 4);
                Value.SetValue(value.M32, offset + 5);
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
                Value.SetValue(value.M11, offset + 00);
                Value.SetValue(value.M12, offset + 01);
                Value.SetValue(value.M13, offset + 02);
                Value.SetValue(value.M14, offset + 03);
                Value.SetValue(value.M21, offset + 04);
                Value.SetValue(value.M22, offset + 05);
                Value.SetValue(value.M23, offset + 06);
                Value.SetValue(value.M24, offset + 07);
                Value.SetValue(value.M31, offset + 08);
                Value.SetValue(value.M32, offset + 09);
                Value.SetValue(value.M33, offset + 10);
                Value.SetValue(value.M34, offset + 11);
                Value.SetValue(value.M41, offset + 12);
                Value.SetValue(value.M42, offset + 13);
                Value.SetValue(value.M43, offset + 14);
                Value.SetValue(value.M44, offset + 15);
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
                Value.SetValue(vec.X, offset + 0);
                Value.SetValue(vec.Y, offset + 1);
                Value.SetValue(vec.Z, offset + 2);
                Value.SetValue(vec.W, offset + 3);
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
