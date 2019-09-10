using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {

        protected override void OnCreated()
        {
            Api = GraphicsApi.OpenGL;
            ApiName = "OpenGL";
        }

        protected override void OnDisplayed()
        {
            GL.Init();

            MaxTextureSize = GL.MaxTextureSize;
            ApiVersion = new Version(GL.MajorVersion, GL.MinorVersion);
        
            base.OnDisplayed();
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

        public override void Target(Target? target)
        {
            GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
        }

        public override void Clear(Color color)
        {
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(GLEnum.COLOR_BUFFER_BIT);
        }
    }
}
