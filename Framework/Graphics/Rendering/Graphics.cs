using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Core Graphics Module, used for Rendering
    /// </summary>
    public abstract class Graphics : AppModule
    {

        /// <summary>
        /// The underlying Graphics API Name
        /// </summary>
        public string ApiName { get; protected set; } = "Unknown";

        /// <summary>
        /// The underlying Graphics API version
        /// </summary>
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);

        /// <summary>
        /// The Underlying Graphics Device Name
        /// </summary>
        public string DeviceName { get; protected set; } = "Unknown";

        /// <summary>
        /// The Maximum Texture Width and Height supported, in pixels
        /// </summary>
        public int MaxTextureSize { get; protected set; } = 0;

        protected Graphics() : base(200)
        {

        }

        protected internal override void Startup()
        {
            Log.Message(Name, $"{ApiName} {ApiVersion} ({DeviceName})");
        }

        /// <summary>
        /// Clears the Color of the Target
        /// </summary>
        public void Clear(RenderTarget target, Color color) => Clear(target, Framework.Clear.Color, color, 0, 0);

        /// <summary>
        /// Clears the Target
        /// </summary>
        public void Clear(RenderTarget target, Color color, float depth, int stencil) => Clear(target, Framework.Clear.All, color, depth, stencil);

        /// <summary>
        /// Clears the Target
        /// </summary>
        public void Clear(RenderTarget target, Clear flags, Color color, float depth, int stencil)
        {
            if (!target.Drawable)
                throw new Exception("Render Target cannot currently be drawn to");

            ClearInternal(target, flags, color, depth, stencil);
        }

        /// <summary>
        /// Clears the Target
        /// </summary>
        protected abstract void ClearInternal(RenderTarget target, Clear flags, Color color, float depth, int stencil);

        /// <summary>
        /// Draws the data from the Render pass to the Render Target.
        /// This will fail if the Target is not Drawable.
        /// </summary>
        public void Render(RenderTarget target, ref RenderPass pass)
        {
            if (!target.Drawable)
                throw new Exception("Render Target cannot currently be drawn to");

            if (!(target is RenderTexture) && !(target is Window))
                throw new Exception("RenderTarget must be a Render Texture or a Window");

            if (pass.Mesh == null)
                throw new Exception("Mesh cannot be null when drawing");

            if (pass.Material == null)
                throw new Exception("Material cannot be null when drawing");

            if (pass.Mesh.InstanceCount > 0 && (pass.Mesh.InstanceFormat == null || (pass.Mesh.InstanceCount < pass.Mesh.InstanceCount)))
                throw new Exception("Trying to draw more Instances than exist in the Mesh");

            if (pass.Mesh.ElementCount < pass.MeshIndexStart + pass.MeshIndexCount)
                throw new Exception("Trying to draw more Elements than exist in the Mesh");

            RenderInternal(target, ref pass);
        }

        protected abstract void RenderInternal(RenderTarget target, ref RenderPass pass);

        /// <summary>
        /// Creates a new Color Texture of the given size
        /// </summary>
        public abstract Texture CreateTexture(int width, int height, TextureFormat format);

        /// <summary>
        /// Creates a new render texture of the given size, with the given amount of color and depth buffers
        /// </summary>
        public abstract RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat);

        /// <summary>
        /// Creates a new Shader from the Shader Source
        /// </summary>
        public abstract Shader CreateShader(ShaderSource source);

        /// <summary>
        /// Creates a new Mesh
        /// </summary>
        public abstract Mesh CreateMesh();



    }
}
