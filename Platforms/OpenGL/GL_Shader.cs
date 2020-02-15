using Foster.Framework;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_Shader : Shader.Platform
    {

        private readonly GL_Graphics graphics;

        public uint ID { get; private set; }

        internal GL_Shader(GL_Graphics graphics, ShaderSource source)
        {
            this.graphics = graphics;

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    Create();
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(null);
                }
            }
            else
            {
                Create();
            }

            void Create()
            {
                ID = GL.CreateProgram();

                Span<uint> shaders = stackalloc uint[2];

                // vertex shader
                if (source.Vertex != null)
                {
                    uint shaderId = GL.CreateShader(GLEnum.VERTEX_SHADER);
                    shaders[0] = shaderId;

                    string glsl = Encoding.UTF8.GetString(source.Vertex);

                    GL.ShaderSource(shaderId, 1, new[] { glsl }, new int[] { glsl.Length });
                    GL.CompileShader(shaderId);

                    string? errorMessage = GL.GetShaderInfoLog(shaderId);
                    if (!string.IsNullOrEmpty(errorMessage))
                        throw new Exception(errorMessage);

                    GL.AttachShader(ID, shaderId);
                }

                // fragment shader
                if (source.Fragment != null)
                {
                    uint shaderId = GL.CreateShader(GLEnum.FRAGMENT_SHADER);
                    shaders[1] = shaderId;

                    string glsl = Encoding.UTF8.GetString(source.Fragment);

                    GL.ShaderSource(shaderId, 1, new[] { glsl }, new int[] { glsl.Length });
                    GL.CompileShader(shaderId);

                    string? errorMessage = GL.GetShaderInfoLog(shaderId);
                    if (!string.IsNullOrEmpty(errorMessage))
                        throw new Exception(errorMessage);

                    GL.AttachShader(ID, shaderId);
                }

                GL.LinkProgram(ID);

                string? programError = GL.GetProgramInfoLog(ID);
                if (!string.IsNullOrEmpty(programError))
                    throw new Exception(programError);

                // get attributes
                GL.GetProgramiv(ID, GLEnum.ACTIVE_ATTRIBUTES, out int attributeCount);
                for (int i = 0; i < attributeCount; i++)
                {
                    GL.GetActiveAttrib(ID, (uint)i, out _, out _, out string name);
                    int location = GL.GetAttribLocation(ID, name);
                    if (location >= 0)
                        Attributes.Add(name, new ShaderAttribute(name, (uint)location));
                }

                // get uniforms
                GL.GetProgramiv(ID, GLEnum.ACTIVE_UNIFORMS, out int uniformCount);
                for (int i = 0; i < uniformCount; i++)
                {
                    GL.GetActiveUniform(ID, (uint)i, out int size, out GLEnum type, out string name);
                    int location = GL.GetUniformLocation(ID, name);
                    if (location >= 0)
                        Uniforms.Add(name, new GL_Uniform(this, name, size, location, type));
                }

                // dispose shaders
                for (int i = 0; i < shaders.Length; i ++)
                {
                    if (shaders[i] != 0)
                    {
                        GL.DetachShader(ID, shaders[i]);
                        GL.DeleteShader(shaders[i]);
                    }
                }
            }
        }

        ~GL_Shader()
        {
            Dispose();
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
                    if (uniform.Type == UniformType.Sampler)
                    {
                        var texture = ((parameter.Value as Texture)?.Implementation as GL_Texture);
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
                    case UniformType.Int:
                        GL.Uniform1i(uniform.Location, (int)(value ?? 0));
                        break;
                    case UniformType.Float:
                        GL.Uniform1f(uniform.Location, (float)(value ?? 0));
                        break;
                    case UniformType.Float2:
                        Vector2 vec2 = (Vector2)(value ?? Vector2.Zero);
                        GL.Uniform2f(uniform.Location, vec2.X, vec2.Y);
                        break;
                    case UniformType.Float3:
                        Vector3 vec3 = (Vector3)(value ?? Vector3.Zero);
                        GL.Uniform3f(uniform.Location, vec3.X, vec3.Y, vec3.Z);
                        break;
                    case UniformType.Float4:
                        Vector4 vec4 = (Vector4)(value ?? Vector4.Zero);
                        GL.Uniform4f(uniform.Location, vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case UniformType.Matrix2D:
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
                    case UniformType.Matrix:
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
                    case UniformType.Sampler:
                        GL.Uniform1i(uniform.Location, (int)(value ?? 0));
                        break;
                }
            }
        }

        protected override void Dispose()
        {
            if (ID != 0)
            {
                graphics.ProgramsToDelete.Add(ID);
                ID = 0;
            }
        }
    }
}
