using System;

namespace Foster.Framework
{
    public abstract class Graphics : Module
    {

        public GraphicsApi Api { get; protected set; } = GraphicsApi.None;
        public string ApiName { get; protected set; } = "Unknown";
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);
        public int MaxTextureSize { get; protected set; } = 0;

        public abstract RectInt Viewport { get; set; }

        public abstract Texture CreateTexture(int width, int height);

        public abstract Target CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false);

        /// <summary>
        /// Creates a new Shader
        /// TODO: This isn't currently api-agnostic ... 
        /// Not sure how to do so without having to write a custom shader language
        /// that gets interpreted depending on the API? (ugh)
        /// </summary>
        public abstract Shader CreateShader(string vertexSource, string fragmentSource);

        public abstract Mesh<T> CreateMesh<T>() where T : struct;

        public abstract void Target(Target? target);
        public abstract void Clear(Color color);
        public abstract void DepthTest(bool enabled);
        public abstract void CullMode(Cull mode);
        public abstract void BlendMode(BlendMode blendMode);

        protected Graphics()
        {
            Priority = 200;
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

        protected internal override void Render(Window window)
        {
            Target(null);
            Clear(Color.Black);
        }
    }
}
