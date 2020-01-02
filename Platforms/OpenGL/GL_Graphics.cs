using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {

        internal List<uint> BuffersToDelete = new List<uint>();
        internal List<uint> ProgramsToDelete = new List<uint>();
        internal List<uint> TexturesToDelete = new List<uint>();
        internal Dictionary<Context, List<uint>> VertexArraysToDelete = new Dictionary<Context, List<uint>>();
        internal Dictionary<Context, List<uint>> FrameBuffersToDelete = new Dictionary<Context, List<uint>>();
        internal Dictionary<Context, RenderState> lastContextRenderState = new Dictionary<Context, RenderState>();

        private readonly List<Context> disposedContexts = new List<Context>();

        private delegate void DeleteResource(uint id);
        private readonly DeleteResource deleteArray = GL.DeleteVertexArray;
        private readonly DeleteResource deleteFramebuffer = GL.DeleteFramebuffer;
        private readonly DeleteResource deleteBuffer = GL.DeleteBuffer;
        private readonly DeleteResource deleteTexture = GL.DeleteTexture;
        private readonly DeleteResource deleteProgram = GL.DeleteProgram;

        internal Context BackgroundContext;

        protected override void Initialized()
        {
            Api = GraphicsApi.OpenGL;
            ApiName = "OpenGL";

            base.Initialized();
        }

        protected override void Startup()
        {
            GL.Init();
            GL.DepthMask(true);

            MaxTextureSize = GL.MaxTextureSize;
            ApiVersion = new Version(GL.MajorVersion, GL.MinorVersion);

            BackgroundContext = App.System.CreateContext();

            base.Startup();
        }

        protected override void Tick()
        {
            // delete any GL graphics resources that are shared between contexts
            DeleteResources(deleteBuffer, BuffersToDelete);
            DeleteResources(deleteProgram, ProgramsToDelete);
            DeleteResources(deleteTexture, TexturesToDelete);

            // check for any resources we're still tracking that are in disposed contexts
            {
                lock (VertexArraysToDelete)
                {
                    foreach (var context in VertexArraysToDelete.Keys)
                        if (context.Disposed)
                            disposedContexts.Add(context);
                }

                lock (FrameBuffersToDelete)
                {
                    foreach (var context in FrameBuffersToDelete.Keys)
                        if (context.Disposed)
                            disposedContexts.Add(context);
                }

                if (disposedContexts.Count > 0)
                {
                    foreach (var context in disposedContexts)
                    {
                        VertexArraysToDelete.Remove(context);
                        FrameBuffersToDelete.Remove(context);
                    }

                    disposedContexts.Clear();
                }
            }
        }

        protected override void AfterRender(WindowTarget target)
        {
            GL.Flush();
        }

        protected override void ContextChanged(Context context)
        {
            DeleteUnusedContextResources(context);
        }

        private void DeleteUnusedContextResources(Context context)
        {
            // delete any VAOs or FrameBuffers associated with this context that need deleting
            if (VertexArraysToDelete.TryGetValue(context, out var vaoList))
                DeleteResources(deleteArray, vaoList);

            if (FrameBuffersToDelete.TryGetValue(context, out var fboList))
                DeleteResources(deleteFramebuffer, fboList);
        }

        private void DeleteResources(DeleteResource deleter, List<uint> list)
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                    deleter(list[i]);
                list.Clear();
            }
        }

        public override Texture CreateTexture(int width, int height, TextureFormat format)
        {
            return new GL_Texture(this, width, height, format, false);
        }

        public override RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            return new GL_RenderTexture(this, width, height, colorAttachmentFormats, depthFormat);
        }

        public override Shader CreateShader(string vertexSource, string fragmentSource)
        {
            return new GL_Shader(this, vertexSource, fragmentSource);
        }

        public override Mesh CreateMesh()
        {
            return new GL_Mesh(this);
        }

        protected override WindowTarget CreateWindowTarget(Window window)
        {
            return new GL_WindowTarget(this, window);
        }

        internal void ApplyRenderState(Context context, ref RenderState state)
        {
            var updateAll = false;
            if (!lastContextRenderState.TryGetValue(context, out var lastState))
                updateAll = true;

            // Blend Mode
            if (updateAll || lastState.BlendMode != state.BlendMode)
            {
                GLEnum op = GetBlendFunc(state.BlendMode.Operation);
                GLEnum src = GetBlendFactor(state.BlendMode.Source);
                GLEnum dst = GetBlendFactor(state.BlendMode.Destination);

                GL.Enable(GLEnum.BLEND);
                GL.BlendEquation(op);
                GL.BlendFunc(src, dst);
            }

            // Depth Function
            if (updateAll || lastState.DepthFunction != state.DepthFunction)
            {
                if (state.DepthFunction == DepthFunctions.None)
                {
                    GL.Disable(GLEnum.DEPTH_TEST);
                }
                else
                {
                    GL.Enable(GLEnum.DEPTH_TEST);

                    switch (state.DepthFunction)
                    {
                        case DepthFunctions.Always:
                            GL.DepthFunc(GLEnum.ALWAYS);
                            break;
                        case DepthFunctions.Equal:
                            GL.DepthFunc(GLEnum.EQUAL);
                            break;
                        case DepthFunctions.Greater:
                            GL.DepthFunc(GLEnum.GREATER);
                            break;
                        case DepthFunctions.GreaterOrEqual:
                            GL.DepthFunc(GLEnum.GEQUAL);
                            break;
                        case DepthFunctions.Less:
                            GL.DepthFunc(GLEnum.LESS);
                            break;
                        case DepthFunctions.LessOrEqual:
                            GL.DepthFunc(GLEnum.LEQUAL);
                            break;
                        case DepthFunctions.Never:
                            GL.DepthFunc(GLEnum.NEVER);
                            break;
                        case DepthFunctions.NotEqual:
                            GL.DepthFunc(GLEnum.NOTEQUAL);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            // Cull Mode
            if (updateAll || lastState.CullMode != state.CullMode)
            {
                if (state.CullMode == Cull.None)
                {
                    GL.Disable(GLEnum.CULL_FACE);
                }
                else
                {
                    GL.Enable(GLEnum.CULL_FACE);
                    if (state.CullMode == Cull.Back)
                    {
                        GL.CullFace(GLEnum.BACK);
                    }
                    else if (state.CullMode == Cull.Front)
                    {
                        GL.CullFace(GLEnum.FRONT);
                    }
                    else
                    {
                        GL.CullFace(GLEnum.FRONT_AND_BACK);
                    }
                }
            }

            // Viewport
            if (updateAll || lastState.Viewport != state.Viewport)
            {
                GL.Viewport(state.Viewport.X, state.Viewport.Y, state.Viewport.Width, state.Viewport.Height);
            }

            // Scissor
            if (updateAll || lastState.Scissor != state.Scissor)
            {
                if (state.Scissor.X <= 0 && state.Scissor.Y <= 0 && state.Scissor.Right >= state.Viewport.Width && state.Scissor.Bottom >= state.Viewport.Height)
                {
                    GL.Disable(GLEnum.SCISSOR_TEST);
                }
                else
                {
                    GL.Enable(GLEnum.SCISSOR_TEST);
                    GL.Scissor(state.Scissor.X, state.Viewport.Height - state.Scissor.Bottom, state.Scissor.Width, state.Scissor.Height);
                }
            }

            lastContextRenderState[context] = state;
        }

        internal void Clear(ClearFlags flags, Color color, float depth, int stencil)
        {
            var mask = GLEnum.ZERO;

            if (flags.HasFlag(ClearFlags.Color))
            {
                GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                mask |= GLEnum.COLOR_BUFFER_BIT;
            }

            if (flags.HasFlag(ClearFlags.Depth))
            {
                GL.ClearDepth(depth);
                mask |= GLEnum.DEPTH_BUFFER_BIT;
            }

            if (flags.HasFlag(ClearFlags.Stencil))
            {
                GL.ClearStencil(stencil);
                mask |= GLEnum.STENCIL_BUFFER_BIT;
            }

            GL.Clear(mask);
        }

        private static GLEnum GetBlendFunc(BlendOperations operation)
        {
            return operation switch
            {
                BlendOperations.Add => GLEnum.FUNC_ADD,
                BlendOperations.Subtract => GLEnum.FUNC_SUBTRACT,
                BlendOperations.ReverseSubtract => GLEnum.FUNC_REVERSE_SUBTRACT,
                BlendOperations.Min => GLEnum.MIN,
                BlendOperations.Max => GLEnum.MAX,

                _ => throw new InvalidOperationException($"Unsupported Blend Opteration {operation}"),
            };
        }

        private static GLEnum GetBlendFactor(BlendFactors factor)
        {
            return factor switch
            {
                BlendFactors.Zero => GLEnum.ZERO,
                BlendFactors.One => GLEnum.ONE,
                BlendFactors.SrcColor => GLEnum.SRC_COLOR,
                BlendFactors.OneMinusSrcColor => GLEnum.ONE_MINUS_SRC_COLOR,
                BlendFactors.DstColor => GLEnum.DST_COLOR,
                BlendFactors.OneMinusDstColor => GLEnum.ONE_MINUS_DST_COLOR,
                BlendFactors.SrcAlpha => GLEnum.SRC_ALPHA,
                BlendFactors.OneMinusSrcAlpha => GLEnum.ONE_MINUS_SRC_ALPHA,
                BlendFactors.DstAlpha => GLEnum.DST_ALPHA,
                BlendFactors.OneMinusDstAlpha => GLEnum.ONE_MINUS_DST_ALPHA,
                BlendFactors.ConstantColor => GLEnum.CONSTANT_COLOR,
                BlendFactors.OneMinusConstantColor => GLEnum.ONE_MINUS_CONSTANT_COLOR,
                BlendFactors.ConstantAlpha => GLEnum.CONSTANT_ALPHA,
                BlendFactors.OneMinusConstantAlpha => GLEnum.ONE_MINUS_CONSTANT_ALPHA,
                BlendFactors.SrcAlphaSaturate => GLEnum.SRC_ALPHA_SATURATE,
                BlendFactors.Src1Color => GLEnum.SRC1_COLOR,
                BlendFactors.OneMinusSrc1Color => GLEnum.ONE_MINUS_SRC1_COLOR,
                BlendFactors.Src1Alpha => GLEnum.SRC1_ALPHA,
                BlendFactors.OneMinusSrc1Alpha => GLEnum.ONE_MINUS_SRC1_ALPHA,

                _ => throw new InvalidOperationException($"Unsupported Blend Factor {factor}"),
            };
        }
    }
}
