using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Collections.Specialized;
using System.Threading;

namespace Foster.OpenGL
{
    public class GL_Shader : InternalShader
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
                    graphics.BackgroundContext.MakeCurrent();
                    CreateShader();
                    GL.Flush();
                    graphics.BackgroundContext.MakeNotCurrent();
                }
            }
            else
            {
                CreateShader();
            }

            void CreateShader()
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

        public void Use(Material material)
        {
            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.BackgroundContext.MakeCurrent();
                    UseShader();
                    GL.Flush();
                    graphics.BackgroundContext.MakeNotCurrent();
                }
            }
            else
            {
                UseShader();
            }

            void UseShader()
            {
                GL.UseProgram(ID);

                // upload uniform values
                int textureSlot = 0;

                foreach (var parameter in material.Parameters.Values)
                {
                    if (!(parameter.Uniform is GL_Uniform uniform))
                        continue;

                    if (uniform.Type == ShaderUniform.Types.Texture2D)
                    {
                        var texture = ((parameter.Value as Texture)?.Internal as GL_Texture);
                        var id = texture?.ID ?? 0;

                        GL.ActiveTexture((uint)(GLEnum.TEXTURE0 + textureSlot));
                        GL.BindTexture(GLEnum.TEXTURE_2D, id);

                        uniform.Upload(textureSlot);
                        textureSlot++;
                    }
                    else
                    {
                        uniform.Upload(parameter.Value);
                    }
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
