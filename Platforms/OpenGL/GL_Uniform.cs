using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    internal class GL_Uniform : ShaderUniform
    {
        public readonly GL_Shader Shader;

        public GL_Uniform(GL_Shader shader, string name, int length, int location, GLEnum type) 
            : base(name, location, length, ToFosterType(type))
        {
            Shader = shader;
        }

        private static UniformType ToFosterType(GLEnum type)
        {
            return type switch
            {
                GLEnum.INT => UniformType.Int,
                GLEnum.FLOAT => UniformType.Float,
                GLEnum.FLOAT_VEC2 => UniformType.Float2,
                GLEnum.FLOAT_VEC3 => UniformType.Float3,
                GLEnum.FLOAT_VEC4 => UniformType.Float4,
                GLEnum.FLOAT_MAT3x2 => UniformType.Matrix3x2,
                GLEnum.FLOAT_MAT4 => UniformType.Matrix4x4,
                GLEnum.SAMPLER_2D => UniformType.Sampler,

                _ => throw new InvalidOperationException("Unknown Enum Type"),
            };
        }
    }
}
