using System;
using Foster.Framework.Internal;

namespace Foster.Framework
{

    public abstract class Graphics : Module
    {

        #region Public API

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
        /// Gets or Sets the current Viewport
        /// </summary>
        public abstract RectInt Viewport { get; set; }

        /// <summary>
        /// Sets the current rendering Target.
        /// Set null to target the current Context backbuffer
        /// </summary>
        public abstract void SetTarget(Target? target);

        /// <summary>
        /// Clears the Color of the current Target
        /// </summary>
        public void ClearColor(Color color) => Clear(ClearFlags.Color, color, 0, 0);

        /// <summary>
        /// Clears the Depth of the current Target
        /// </summary>
        /// <param name="depth"></param>
        public void ClearDepth(float depth) => Clear(ClearFlags.Depth, 0, depth, 0);

        /// <summary>
        /// Clears the Stencil Buffer of the current Target
        /// </summary>
        /// <param name="stencil"></param>
        public void ClearStencil(int stencil) => Clear(ClearFlags.Stencil, 0, 0, stencil);

        /// <summary>
        /// Clears the current Target
        /// </summary>
        public void Clear(Color color, float depth, int stencil) => Clear(ClearFlags.All, color, depth, stencil);

        /// <summary>
        /// Clears the current Target to the given color
        /// </summary>
        public abstract void Clear(ClearFlags flags, Color color, float depth, int stencil);

        /// <summary>
        /// Enables or Disables Depth-testing
        /// </summary>
        public abstract void SetDepthTest(bool enabled);

        /// <summary>
        /// Sets the Depth Testing function
        /// </summary>
        public abstract void SetDepthFunction(DepthFunctions func);

        /// <summary>
        /// Sets the current Culling Mode
        /// </summary>
        public abstract void SetCullMode(Cull mode);

        /// <summary>
        /// Sets the current Blend Mode
        /// </summary>
        public abstract void SetBlendMode(BlendMode blendMode);

        /// <summary>
        /// Enables and Sets the current Scissor Rectangle
        /// This is from the Top-Left of the rendering target
        /// </summary>
        /// <param name="scissor"></param>
        public abstract void SetScissor(RectInt scissor);

        /// <summary>
        /// Disables the Scissor Rectangle
        /// </summary>
        public abstract void DisableScissor();

        #endregion

        #region Internal API

        /// <summary>
        /// Creates a new Color Texture of the given size
        /// </summary>
        protected internal abstract InternalTexture CreateTexture(int width, int height, TextureFormat format);

        /// <summary>
        /// Creates a new render target of the given size, with the given amount of color and depth buffers
        /// </summary>
        protected internal abstract InternalTarget CreateTarget(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat);

        /// <summary>
        /// Creates a new Shader
        /// </summary>
        protected internal abstract InternalShader CreateShader(string vertexSource, string fragmentSource);

        /// <summary>
        /// Creates a new Mesh
        /// </summary>
        protected internal abstract InternalMesh CreateMesh();

        #endregion

        protected Graphics() : base(200)
        {

        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

        protected internal override void BeforeRender(Window window)
        {
            SetTarget(null);
        }
    }
}
