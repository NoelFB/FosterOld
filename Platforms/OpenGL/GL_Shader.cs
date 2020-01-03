using Foster.Framework;
using System;
using System.Collections.Specialized;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_Shader : Shader
    {

        private readonly GL_Graphics graphics;

        public uint ID { get; private set; }

        internal GL_Shader(GL_Graphics graphics, string vertexSource, string fragmentSource)
        {
            this.graphics = graphics;

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.Device.SetCurrentContext(graphics.BackgroundContext);

                    Create();
                    GL.Flush();

                    graphics.Device.SetCurrentContext(null);
                }
            }
            else
            {
                Create();
            }

            void Create()
            {
                // create vertex shader
                uint vertex = GL.CreateShader(GLEnum.VERTEX_SHADER);
                {
                    GL.ShaderSource(vertex, 1, new[] { vertexSource }, new int[] { vertexSource.Length });
                    GL.CompileShader(vertex);

                    string? vertexError = GL.GetShaderInfoLog(vertex);
                    if (!string.IsNullOrEmpty(vertexError))
                        throw new Exception(vertexError);
                }

                // create fragment shader
                uint fragment = GL.CreateShader(GLEnum.FRAGMENT_SHADER);
                {
                    GL.ShaderSource(fragment, 1, new[] { fragmentSource }, new int[] { fragmentSource.Length });
                    GL.CompileShader(fragment);

                    string? fragmentError = GL.GetShaderInfoLog(fragment);
                    if (!string.IsNullOrEmpty(fragmentError))
                        throw new Exception(fragmentError);
                }

                // create program
                ID = GL.CreateProgram();
                {
                    GL.AttachShader(ID, vertex);
                    GL.AttachShader(ID, fragment);
                    GL.LinkProgram(ID);

                    string? programError = GL.GetProgramInfoLog(ID);
                    if (!string.IsNullOrEmpty(programError))
                        throw new Exception(programError);
                }

                // get attributes
                GL.GetProgramiv(ID, GLEnum.ACTIVE_ATTRIBUTES, out int attributeCount);
                for (int i = 0; i < attributeCount; i++)
                {
                    GL.GetActiveAttrib(ID, (uint)i, out _, out _, out string name);
                    int location = GL.GetAttribLocation(ID, name);
                    if (location >= 0)
                        attributes.Add(name, new ShaderAttribute(name, (uint)location));
                }

                // get uniforms
                GL.GetProgramiv(ID, GLEnum.ACTIVE_UNIFORMS, out int uniformCount);
                for (int i = 0; i < uniformCount; i++)
                {
                    GL.GetActiveUniform(ID, (uint)i, out int size, out GLEnum type, out string name);
                    int location = GL.GetUniformLocation(ID, name);
                    if (location >= 0)
                        uniforms.Add(name, new GL_Uniform(this, name, size, location, type));
                }

                // dispose fragment and vertex shaders
                GL.DetachShader(ID, vertex);
                GL.DetachShader(ID, fragment);
                GL.DeleteShader(vertex);
                GL.DeleteShader(fragment);
            }
        }

        ~GL_Shader()
        {
            DisposeResources();
        }

        public unsafe void Use(Material material)
        {
            GL.UseProgram(ID);

            // upload uniform values
            int textureSlot = 0;

            foreach (var parameter in material.Parameters.Values)
            {
                if (!(parameter.Uniform is GL_Uniform uniform))
                    continue;

                // get the uniform value
                object? value;
                {
                    if (uniform.Type == ShaderUniform.Types.Texture2D)
                    {
                        var texture = (parameter.Value as GL_Texture);
                        var id = texture?.ID ?? 0;

                        GL.ActiveTexture((uint)(GLEnum.TEXTURE0 + textureSlot));
                        GL.BindTexture(GLEnum.TEXTURE_2D, id);

                        value = textureSlot;
                        textureSlot++;
                    }
                    else
                    {
                        value = parameter.Value;
                    }
                }

                // check if it's different
                if (uniform.LastValue == value)
                    continue;
                uniform.LastValue = value;

                // upload it
                switch (uniform.Type)
                {
                    case ShaderUniform.Types.Int:
                        GL.Uniform1i(uniform.Location, (int)(value ?? 0));
                        break;
                    case ShaderUniform.Types.Float:
                        GL.Uniform1f(uniform.Location, (float)(value ?? 0));
                        break;
                    case ShaderUniform.Types.Float2:
                        Vector2 vec2 = (Vector2)(value ?? Vector2.Zero);
                        GL.Uniform2f(uniform.Location, vec2.X, vec2.Y);
                        break;
                    case ShaderUniform.Types.Float3:
                        Vector3 vec3 = (Vector3)(value ?? Vector3.Zero);
                        GL.Uniform3f(uniform.Location, vec3.X, vec3.Y, vec3.Z);
                        break;
                    case ShaderUniform.Types.Float4:
                        Vector4 vec4 = (Vector4)(value ?? Vector4.Zero);
                        GL.Uniform4f(uniform.Location, vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case ShaderUniform.Types.Matrix2D:
                        {
                            Matrix2D m3x2 = (Matrix2D)(value ?? Matrix2D.Identity);
                            float* matrix = stackalloc float[6];

                            matrix[0] = m3x2.M11;
                            matrix[1] = m3x2.M12;
                            matrix[2] = m3x2.M21;
                            matrix[3] = m3x2.M22;
                            matrix[4] = m3x2.M31;
                            matrix[5] = m3x2.M32;

                            GL.UniformMatrix3x2fv(uniform.Location, 1, false, new IntPtr(matrix));
                        }
                        break;
                    case ShaderUniform.Types.Matrix:
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

                            GL.UniformMatrix4fv(uniform.Location, 1, false, new IntPtr(matrix));
                        }
                        break;
                    case ShaderUniform.Types.Texture2D:
                        GL.Uniform1i(uniform.Location, (int)(value ?? 0));
                        break;
                }
            }
        }

        protected override void DisposeResources()
        {
            if (ID != 0)
            {
                graphics.ProgramsToDelete.Add(ID);
                ID = 0;
            }
        }
    }
}
