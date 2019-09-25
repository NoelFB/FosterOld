using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_Shader : Shader
    {

        public readonly uint ID;
        internal bool dirty = true;

        public GL_Shader(GL_Graphics graphics, string vertexSource, string fragmentSource) : base(graphics)
        {
            // create vertex shader
            uint vertex = GL.CreateShader(GLEnum.VERTEX_SHADER);
            {
                GL.ShaderSource(vertex, 1, new string[] { vertexSource }, new int[] { vertexSource.Length });
                GL.CompileShader(vertex);

                string? vertexError = GL.GetShaderInfoLog(vertex);
                if (!string.IsNullOrEmpty(vertexError))
                {
                    throw new Exception(vertexError);
                }
            }

            // create fragment shader
            uint fragment = GL.CreateShader(GLEnum.FRAGMENT_SHADER);
            {
                GL.ShaderSource(fragment, 1, new string[] { fragmentSource }, new int[] { fragmentSource.Length });
                GL.CompileShader(fragment);

                string? fragmentError = GL.GetShaderInfoLog(fragment);
                if (!string.IsNullOrEmpty(fragmentError))
                {
                    throw new Exception(fragmentError);
                }
            }

            // create program
            ID = GL.CreateProgram();
            {
                GL.AttachShader(ID, vertex);
                GL.AttachShader(ID, fragment);
                GL.LinkProgram(ID);

                string? programError = GL.GetProgramInfoLog(ID);
                if (!string.IsNullOrEmpty(programError))
                {
                    throw new Exception(programError);
                }
            }

            // get uniforms
            GL.GetProgramiv(ID, GLEnum.ACTIVE_UNIFORMS, out int uniformCount);
            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(ID, (uint)i, out int size, out GLEnum type, out string name);
                int location = GL.GetUniformLocation(ID, name);

                GL_ShaderUniform uniform = new GL_ShaderUniform(this, name, size, location, type);
                uniforms.Add(name, uniform);

                if (uniform.Type == UniformType.Texture2D)
                {
                    textures.Add(uniform);
                }
            }

            // dispose fragment and vertex shaders
            GL.DetachShader(ID, vertex);
            GL.DetachShader(ID, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public override void Use()
        {
            GL.UseProgram(ID);

            // always rebind textures
            for (int i = 0; i < textures.Count; i++)
            {
                uint id = (textures[i].Value as GL_Texture)?.ID ?? 0;

                GL.ActiveTexture((uint)(GLEnum.TEXTURE0 + i));
                GL.BindTexture(GLEnum.TEXTURE_2D, id);
            }

            // upload uniform values
            if (dirty)
            {
                int textureSlot = 0;

                foreach (ShaderUniform uni in uniforms.Values)
                {
                    if (!(uni is GL_ShaderUniform uniform))
                    {
                        continue;
                    }

                    if (!uniform.Dirty)
                    {
                        continue;
                    }

                    if (uniform.Type == UniformType.Texture2D)
                    {
                        uniform.Upload(textureSlot);
                        textureSlot++;
                    }
                    else
                    {
                        uniform.Upload();
                    }
                }

                dirty = false;
            }
        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                if (Graphics is GL_Graphics graphics)
                    graphics.ProgramsToDelete.Add(ID);
            }

            base.Dispose();
        }
    }
}
