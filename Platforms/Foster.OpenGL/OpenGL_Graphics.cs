using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class OpenGL_Graphics : Graphics
    {

        protected override void Created()
        {
            Api = GraphicsApi.OpenGL;
            ApiName = "OpenGL";
            ApiVersion = new Version();
            MaxTextureSize = 8192;
        }

        public override Texture CreateTexture(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Target CreateTarget(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Shader CreateShader(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Mesh<T> CreateMesh<T>()
        {
            throw new NotImplementedException();
        }


    }
}
