using System;
using Foster.Framework.Internal;

namespace Foster.Framework
{

    public abstract class Graphics : Module
    {

        /// <summary>
        /// The underlying Graphics API
        /// </summary>
        public GraphicsApi Api { get; protected set; } = GraphicsApi.None;

        /// <summary>
        /// The underlying Graphics API Name
        /// </summary>
        public string ApiName { get; protected set; } = "Unknown";

        /// <summary>
        /// The underlying Graphics API version
        /// </summary>
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);

        /// <summary>
        /// The Maximum Texture Width and Height supported, in pixels
        /// </summary>
        public int MaxTextureSize { get; protected set; } = 0;

        /// <summary>
        /// Creates a new Color Texture of the given size
        /// </summary>
        public abstract Texture CreateTexture(int width, int height, TextureFormat format);

        /// <summary>
        /// Creates a new render texture of the given size, with the given amount of color and depth buffers
        /// </summary>
        public abstract RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat);

        /// <summary>
        /// Creates a new Shader
        /// 
        /// TODO: this isn't api-agnostic and is not how the true implementation should work
        /// see Shader.cs for more thoughts
        /// 
        /// </summary>
        public abstract Shader CreateShader(string vertexSource, string fragmentSource);

        /// <summary>
        /// Creates a new Mesh
        /// </summary>
        public abstract Mesh CreateMesh();

        /// <summary>
        /// Creates a Window Target for the given Window
        /// </summary>
        protected internal abstract WindowTarget CreateWindowTarget(Window window);

        protected Graphics() : base(200)
        {

        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }
    }
}
