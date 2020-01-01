using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_Uniform : ShaderUniform
    {
        public readonly GL_Shader Shader;
        private object? lastValue;

        public GL_Uniform(GL_Shader shader, string name, int size, int location, GLEnum type) 
            : base(name, location, size, ToFosterType(type))
        {
            Shader = shader;
        }

        public unsafe void Upload(object? value)
        {
            if (lastValue == value)
                return;
            lastValue = value;

            switch (Type)
            {
                case Types.Int:
                    GL.Uniform1i(Location, (int)(value ?? 0));
                    break;
                case Types.Float:
                    GL.Uniform1f(Location, (float)(value ?? 0));
                    break;
                case Types.Float2:
                    Vector2 vec2 = (Vector2)(value ?? Vector2.Zero);
                    GL.Uniform2f(Location, vec2.X, vec2.Y);
                    break;
                case Types.Float3:
                    Vector3 vec3 = (Vector3)(value ?? Vector3.Zero);
                    GL.Uniform3f(Location, vec3.X, vec3.Y, vec3.Z);
                    break;
                case Types.Float4:
                    Vector4 vec4 = (Vector4)(value ?? Vector4.Zero);
                    GL.Uniform4f(Location, vec4.X, vec4.Y, vec4.Z, vec4.W);
                    break;
                case Types.Matrix2D:
                    {
                        Matrix2D m3x2 = (Matrix2D)(value ?? Matrix2D.Identity);
                        float* matrix = stackalloc float[6];

                        matrix[0] = m3x2.M11;
                        matrix[1] = m3x2.M12;
                        matrix[2] = m3x2.M21;
                        matrix[3] = m3x2.M22;
                        matrix[4] = m3x2.M31;
                        matrix[5] = m3x2.M32;

                        GL.UniformMatrix3x2fv(Location, 1, false, new IntPtr(matrix));
                    }
                    break;
                case Types.Matrix:
                    {
                        float* matrix = stackalloc float[16];

                        if (value is Matrix2D m3x2)
                        {
                            matrix[00] = m3x2.M11;
                            matrix[01] = m3x2.M12;
                            matrix[02] = 0f;
                            matrix[03] = 0f;
                            matrix[04] = m3x2.M21;
                            matrix[05] = m3x2.M22;
                            matrix[06] = 0f;
                            matrix[07] = 0f;
                            matrix[08] = 0f;
                            matrix[09] = 0f;
                            matrix[10] = 1f;
                            matrix[11] = 0f;
                            matrix[12] = m3x2.M31;
                            matrix[13] = m3x2.M32;
                            matrix[14] = 0f;
                            matrix[15] = 1f;
                        }
                        else if (value is Matrix m4x4)
                        {
                            // TODO:
                            // optimize this out? create pointer to struct and just send that?

                            matrix[00] = m4x4.M11;
                            matrix[01] = m4x4.M12;
                            matrix[02] = m4x4.M13;
                            matrix[03] = m4x4.M14;
                            matrix[04] = m4x4.M21;
                            matrix[05] = m4x4.M22;
                            matrix[06] = m4x4.M23;
                            matrix[07] = m4x4.M24;
                            matrix[08] = m4x4.M31;
                            matrix[09] = m4x4.M32;
                            matrix[10] = m4x4.M33;
                            matrix[11] = m4x4.M34;
                            matrix[12] = m4x4.M41;
                            matrix[13] = m4x4.M42;
                            matrix[14] = m4x4.M43;
                            matrix[15] = m4x4.M44;
                        }

                        GL.UniformMatrix4fv(Location, 1, false, new IntPtr(matrix));
                    }
                    break;
                case Types.Texture2D:
                    GL.Uniform1i(Location, (int)(value ?? 0));
                    break;
            }
        }

        private static Types ToFosterType(GLEnum type)
        {
            return type switch
            {
                GLEnum.INT => Types.Int,
                GLEnum.FLOAT => Types.Float,
                GLEnum.FLOAT_VEC2 => Types.Float2,
                GLEnum.FLOAT_VEC3 => Types.Float3,
                GLEnum.FLOAT_VEC4 => Types.Float4,
                GLEnum.FLOAT_MAT3x2 => Types.Matrix2D,
                GLEnum.FLOAT_MAT4 => Types.Matrix,
                GLEnum.SAMPLER_2D => Types.Texture2D,

                _ => throw new InvalidOperationException("Unknown Enum Type"),
            };
        }
    }
}
