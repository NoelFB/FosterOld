using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_ShaderUniform : ShaderUniform
    {
        public readonly GL_Shader Shader;
        public readonly int Size;

        public GL_ShaderUniform(GL_Shader shader, string name, int size, int location, GLEnum type)
        {
            Shader = shader;
            Name = name;
            Size = size;
            Location = location;

            switch (type)
            {
                case GLEnum.INT: Type = UniformType.Int; break;
                case GLEnum.FLOAT: Type = UniformType.Float; break;
                case GLEnum.FLOAT_VEC2: Type = UniformType.Float2; break;
                case GLEnum.FLOAT_VEC3: Type = UniformType.Float3; break;
                case GLEnum.FLOAT_VEC4: Type = UniformType.Float4; break;
                case GLEnum.FLOAT_MAT3x2: Type = UniformType.Matrix3x2; break;
                case GLEnum.FLOAT_MAT4: Type = UniformType.Matrix4x4; break;
                case GLEnum.SAMPLER_2D: default: Type = UniformType.Texture2D; break;
            }
        }

        public unsafe void Upload(object? value)
        {
            switch (Type)
            {
                case UniformType.Int:
                    GL.Uniform1i(Location, (int)(value ?? 0));
                    break;
                case UniformType.Float:
                    GL.Uniform1f(Location, (float)(value ?? 0));
                    break;
                case UniformType.Float2:
                    Vector2 vec2 = (Vector2)(value ?? Vector2.Zero);
                    GL.Uniform2f(Location, vec2.X, vec2.Y);
                    break;
                case UniformType.Float3:
                    Vector3 vec3 = (Vector3)(value ?? Vector3.Zero);
                    GL.Uniform3f(Location, vec3.X, vec3.Y, vec3.Z);
                    break;
                case UniformType.Float4:
                    Vector4 vec4 = (Vector4)(value ?? Vector4.Zero);
                    GL.Uniform4f(Location, vec4.X, vec4.Y, vec4.Z, vec4.W);
                    break;
                case UniformType.Matrix3x2:
                    {
                        Matrix3x2 m3x2 = (Matrix3x2)(value ?? Matrix3x2.Identity);
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
                case UniformType.Matrix4x4:
                    {
                        float* matrix = stackalloc float[16];

                        if (value is Matrix3x2 m3x2)
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
                        else if (value is Matrix4x4 m4x4)
                        {
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
                case UniformType.Texture2D:
                    GL.Uniform1i(Location, (int)(value ?? 0));
                    break;
            }
        }
    }
}
