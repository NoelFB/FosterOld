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
        /// Gets or Sets the current Viewport
        /// </summary>
        public abstract RectInt Viewport { get; set; }

        /// <summary>
        /// Creates a new Color Texture of the given size
        /// </summary>
        public abstract Texture CreateTexture(int width, int height);

        /// <summary>
        /// Creates a new render target of the given size, with the given amount of textures and depth buffer associated
        /// </summary>
        public abstract Target CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false);

        /// <summary>
        /// Creates a new Shader
        /// 
        /// TODO: This isn't api-agnostic ... 
        /// Not sure what the best way to implement this is:
        /// 
        ///     1)  Create our own Shader "language" that each platform can convert
        ///         into their required language. This has a lot of drawbacks
        ///         because it needs to be very easy to parse somehow ...
        ///         
        ///     2)  Require either GLSL or HLSL and use a 3rd party tool to convert them.
        ///         The problem here is that I don't want to add more dependencies
        /// 
        /// </summary>
        public abstract Shader CreateShader(string vertexSource, string fragmentSource);

        /// <summary>
        /// Creates a new Mesh with the Vertex type T
        /// </summary>
        public abstract Mesh<T> CreateMesh<T>() where T : struct;

        /// <summary>
        /// Sets the current rendering Target.
        /// Set null to target the current Context backbuffer
        /// </summary>
        public abstract void Target(Target? target);

        public void ClearColor(Color color)
        {
            Clear(ClearFlags.Color, color, 0, 0);
        }

        public void ClearDepth(float depth)
        {
            Clear(ClearFlags.Depth, 0, depth, 0);
        }

        public void ClearStencil(int stencil)
        {
            Clear(ClearFlags.Stencil, 0, 0, stencil);
        }

        /// <summary>
        /// Clears the current Target to the given color
        /// </summary>
        public abstract void Clear(ClearFlags flags, Color color, float depth, int stencil);

        /// <summary>
        /// Enables or Disables Depth-testing
        /// </summary>
        public abstract void DepthTest(bool enabled);

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
