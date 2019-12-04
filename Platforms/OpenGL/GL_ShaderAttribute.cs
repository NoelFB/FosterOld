using Foster.Framework;

namespace Foster.OpenGL
{
    public class GL_ShaderAttribute : ShaderAttribute
    {
        public readonly GL_Shader Shader;

        public GL_ShaderAttribute(GL_Shader shader, string name, uint location)
        {
            Shader = shader;
            Name = name;
            Location = location;
        }
    }
}
