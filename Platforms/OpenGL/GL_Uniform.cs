using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    internal class GL_Uniform : ShaderUniform
    {
        public readonly GL_Shader Shader;
        public object? LastValue;

        public GL_Uniform(GL_Shader shader, string name, int size, int location, GLEnum type) 
            : base(name, location, size, ToFosterType(type))
        {
            Shader = shader;
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
