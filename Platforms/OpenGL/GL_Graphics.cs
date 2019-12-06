using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Collections.Generic;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {

        internal List<uint> BuffersToDelete = new List<uint>();
        internal List<uint> ProgramsToDelete = new List<uint>();
        internal List<uint> TexturesToDelete = new List<uint>();
        internal Dictionary<Context, List<uint>> VertexArraysToDelete = new Dictionary<Context, List<uint>>();
        internal Dictionary<Context, List<uint>> FrameBuffersToDelete = new Dictionary<Context, List<uint>>();

        private readonly List<Context> disposedContexts = new List<Context>();

        private delegate void DeleteResource(uint id);
        private readonly DeleteResource deleteArray = GL.DeleteVertexArray;
        private readonly DeleteResource deleteFramebuffer = GL.DeleteFramebuffer;
        private readonly DeleteResource deleteBuffer = GL.DeleteBuffer;
        private readonly DeleteResource deleteTexture = GL.DeleteTexture;
        private readonly DeleteResource deleteProgram = (id) => GL.DeleteProgram(id);

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

        protected override void AfterRender(Window window)
        {
            GL.Flush();

            DeleteUnusedContextResources(window.Context);
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

        private unsafe void DeleteResources(DeleteResource deleter, List<uint> list)
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                    deleter(list[i]);
                list.Clear();
            }
        }

        private RectInt viewport;

        public override RectInt Viewport
        {
            get => viewport;
            set
            {
                viewport = value;
                GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            }
        }

        protected override InternalTexture CreateTexture(int width, int height)
        {
            return new GL_Texture(this, width, height);
        }

        protected override InternalTarget CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false, bool stencilBuffer = false)
        {
            return new GL_Target(this, width, height, textures, depthBuffer, stencilBuffer);
        }

        protected override InternalShader CreateShader(string vertexSource, string fragmentSource)
        {
            return new GL_Shader(this, vertexSource, fragmentSource);
        }

        protected override InternalMesh CreateMesh()
        {
            return new GL_Mesh(this);
        }

        public override void SetTarget(Target? target)
        {
            if (target != null && target.Internal is GL_Target glTarget)
            {
                glTarget.Bind();
                Viewport = new RectInt(0, 0, target.Width, target.Height);
            }
            else
            {
                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);

                var context = App.System.GetCurrentContext();
                if (context != null)
                    Viewport = new RectInt(0, 0, context.Width, context.Height);
            }

            DisableScissor();
        }

        public override void SetDepthTest(bool enabled)
        {
            if (enabled)
            {
                GL.Enable(GLEnum.DEPTH_TEST);
            }
            else
            {
                GL.Disable(GLEnum.DEPTH_TEST);
            }
        }

        public override void SetDepthFunction(DepthFunctions func)
        {
            switch (func)
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

        public override void SetCullMode(Cull mode)
        {
            if (mode == Cull.None)
            {
                GL.Disable(GLEnum.CULL_FACE);
            }
            else
            {
                GL.Enable(GLEnum.CULL_FACE);
                if (mode == Cull.Back)
                {
                    GL.CullFace(GLEnum.BACK);
                }
                else if (mode == Cull.Front)
                {
                    GL.CullFace(GLEnum.FRONT);
                }
                else
                {
                    GL.CullFace(GLEnum.FRONT_AND_BACK);
                }
            }
        }

        public override void SetBlendMode(BlendMode blendMode)
        {
            GLEnum op = GetBlendFunc(blendMode.Operation);
            GLEnum src = GetBlendFactor(blendMode.Source);
            GLEnum dst = GetBlendFactor(blendMode.Destination);

            GL.Enable(GLEnum.BLEND);
            GL.BlendEquation(op);
            GL.BlendFunc(src, dst);
        }

        public override void Clear(ClearFlags flags, Color color, float depth, int stencil)
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

        public override void SetScissor(RectInt scissor)
        {
            GL.Enable(GLEnum.SCISSOR_TEST);
            GL.Scissor(scissor.X, viewport.Height - scissor.Bottom, scissor.Width, scissor.Height);
        }

        public override void DisableScissor()
        {
            GL.Disable(GLEnum.SCISSOR_TEST);
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

                _ => throw new Exception($"Unsupported Blend Opteration {operation}"),
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

                _ => throw new Exception($"Unsupported Blend Factor {factor}"),
            };
        }
    }
}
