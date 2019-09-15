using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Graphics : Module
    {

        public GraphicsApi Api { get; protected set; }
        public string? ApiName { get; protected set; }
        public Version? ApiVersion { get; protected set; }
        public int MaxTextureSize { get; protected set; }

        public abstract RectInt Viewport { get; set; }

        public abstract Texture CreateTexture(int width, int height);
        public abstract Target CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false);
        public abstract Shader CreateShader(string vertexSource, string fragmentSource);
        public abstract Mesh<T> CreateMesh<T>() where T : struct;

        public abstract void Target(Target? target);
        public abstract void Clear(Color color);
        public abstract void DepthTest(bool enabled);
        public abstract void CullMode(Cull mode);
        public abstract void BlendMode(BlendMode blendMode);

        protected internal override void OnDisplayed()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

    }
}
