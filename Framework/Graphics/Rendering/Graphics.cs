using System;

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

        protected Graphics() : base(200)
        {

        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

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

        /// <summary>
        /// Draws the given Render Pass
        /// </summary>
        public void Draw(ref RenderPass renderPass)
        {
            if (renderPass.Target == null || !renderPass.Target.Drawable)
                throw new Exception("Target is not Drawable");

            if (renderPass.Mesh == null)
                throw new Exception("Mesh cannot be null when drawing");

            if (renderPass.Material == null)
                throw new Exception("Material cannot be null when drawing");

            if (renderPass.Mesh.InstanceCount > 0 && (renderPass.Mesh.InstanceFormat == null || (renderPass.Mesh.InstanceCount < renderPass.Mesh.InstanceCount)))
                throw new Exception("Trying to draw more Instances than exist in the Mesh");

            if (renderPass.Mesh.ElementCount < renderPass.MeshStartElement + renderPass.MeshElementCount)
                throw new Exception("Trying to draw more Elements than exist in the Mesh");

            PerformDraw(ref renderPass);
        }

        /// <summary>
        /// Draws the given Render Pass
        /// </summary>
        protected abstract void PerformDraw(ref RenderPass drawState);

    }
}
