using System;

namespace Foster.Framework
{
    [Flags]
    public enum ClearFlags
    {
        None = 0,
        Color = 1,
        Depth = 2,
        Stencil = 4,
        All = 7
    }

    public enum DepthFunctions
    {
        /// <summary>
        /// The depth test always passes.
        /// </summary>
        Always,

        /// <summary>
        /// The depth test never passes.
        /// </summary>
        Never,

        /// <summary>
        /// Passes if the fragment's depth value is less than the stored depth value.
        /// </summary>
        Less,

        /// <summary>
        /// Passes if the fragment's depth value is equal to the stored depth value.
        /// </summary>
        Equal,

        /// <summary>
        /// Passes if the fragment's depth value is less than or equal to the stored depth value.
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// Passes if the fragment's depth value is greater than the stored depth value.
        /// </summary>
        Greater,

        /// <summary>
        /// Passes if the fragment's depth value is not equal to the stored depth value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Passes if the fragment's depth value is greater than or equal to the stored depth value.
        /// </summary>
        GreaterOrEqual
    }

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
        public abstract void Target(Target? target);

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
        public abstract void DepthTest(bool enabled);

        /// <summary>
        /// Sets the Depth Testing function
        /// </summary>
        public abstract void DepthFunction(DepthFunctions func);

        /// <summary>
        /// Sets the current Culling Mode
        /// </summary>
        public abstract void CullMode(Cull mode);

        /// <summary>
        /// Sets the current Blend Mode
        /// </summary>
        public abstract void BlendMode(BlendMode blendMode);

        /// <summary>
        /// Enables and Sets the current Scissor Rectangle
        /// This is from the Top-Left of the rendering target
        /// </summary>
        /// <param name="scissor"></param>
        public abstract void Scissor(RectInt scissor);

        /// <summary>
        /// Disables the Scissor Rectangle
        /// </summary>
        public abstract void DisableScissor();

        #endregion

        #region Internal API

        /// <summary>
        /// Creates a new Color Texture of the given size
        /// </summary>
        protected internal abstract InternalTexture CreateTexture(int width, int height);

        /// <summary>
        /// Creates a new render target of the given size, with the given amount of color, depth, and stencil buffers
        /// </summary>
        protected internal abstract InternalTarget CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false, bool stencilBuffer = false);

        /// <summary>
        /// Creates a new Shader
        /// </summary>
        protected internal abstract InternalShader CreateShader(string vertexSource, string fragmentSource);

        /// <summary>
        /// Creates a new Mesh
        /// </summary>
        protected internal abstract InternalMesh CreateMesh();

        #endregion

        protected Graphics()
        {
            Priority = 200;
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

        protected internal override void BeforeRender(Window window)
        {
            Target(null);
        }
    }
}
