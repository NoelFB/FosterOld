using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {
        // The Background Context isn't created until the Startup Event, but afterwards it's never null
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        internal RenderingContext BackgroundContext { get; private set; }
        internal RenderingState RenderingState { get; private set; }
#pragma warning restore CS8618 

        internal List<uint> BuffersToDelete = new List<uint>();
        internal List<uint> ProgramsToDelete = new List<uint>();
        internal List<uint> TexturesToDelete = new List<uint>();
        internal Dictionary<RenderingContext, List<uint>> VertexArraysToDelete = new Dictionary<RenderingContext, List<uint>>();
        internal Dictionary<RenderingContext, List<uint>> FrameBuffersToDelete = new Dictionary<RenderingContext, List<uint>>();

        private delegate void DeleteResource(uint id);
        private readonly DeleteResource deleteArray = GL.DeleteVertexArray;
        private readonly DeleteResource deleteFramebuffer = GL.DeleteFramebuffer;
        private readonly DeleteResource deleteBuffer = GL.DeleteBuffer;
        private readonly DeleteResource deleteTexture = GL.DeleteTexture;
        private readonly DeleteResource deleteProgram = GL.DeleteProgram;

        private readonly HashSet<RenderingContext> tempRemovingList = new HashSet<RenderingContext>();

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

            RenderingState = GetRenderingState(App.System);
            BackgroundContext = RenderingState.CreateContext();

            base.Startup();
        }

        protected override void Tick()
        {
            // delete any GL graphics resources that are shared between contexts
            DeleteResources(deleteBuffer, BuffersToDelete);
            DeleteResources(deleteProgram, ProgramsToDelete);
            DeleteResources(deleteTexture, TexturesToDelete);

            // delete resources tied to specific contexts
            if (VertexArraysToDelete.Count > 0 || FrameBuffersToDelete.Count > 0)
            {
                var currentContext = RenderingState.GetCurrentContext();

                DeleteContextResources(VertexArraysToDelete, deleteArray);
                DeleteContextResources(FrameBuffersToDelete, deleteFramebuffer);

                RenderingState.SetCurrentContext(currentContext);
            }
        }

        protected override void AfterRender(Window window)
        {
            GL.Flush();
        }

        private void DeleteContextResources(Dictionary<RenderingContext, List<uint>> map, DeleteResource deleter)
        {
            lock (map)
            {
                foreach (var kv in map)
                {
                    var context = kv.Key;
                    var list = kv.Value;

                    if (!context.Disposed && list.Count > 0)
                    {
                        if (context.ActiveThreadId == 0)
                            context.MakeCurrent();

                        if (context.ActiveThreadId == Thread.CurrentThread.ManagedThreadId)
                        {
                            DeleteResources(deleter, list);
                            tempRemovingList.Add(context);
                        }
                    }
                    else
                        tempRemovingList.Add(context);
                }

                foreach (var ctx in tempRemovingList)
                    map.Remove(ctx);
            }
        }

        private void DeleteResources(DeleteResource deleter, List<uint> list)
        {
            for (int i = 0; i < list.Count; i++)
                deleter(list[i]);
            list.Clear();
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

        protected override InternalTexture CreateTexture(int width, int height, TextureFormat format)
        {
            return new GL_Texture(this, width, height, format);
        }

        protected override InternalTarget CreateTarget(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            return new GL_Target(this, width, height, colorAttachmentFormats, depthFormat);
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

                var context = RenderingState.GetCurrentContext();
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
