using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {
        // The Background Context can be null up until Startup, at which point it never is again
#pragma warning disable CS8618
        internal Context BackgroundContext;
#pragma warning restore CS8618

        // Stores info about the Context
        internal class ContextMeta
        {
            public List<uint> VertexArraysToDelete = new List<uint>();
            public List<uint> FrameBuffersToDelete = new List<uint>();
            public RenderPass? LastRenderState;
            public RectInt? LastViewport;
            public bool ForceScissorUpdate;
        }

        // various resources waiting to be deleted
        internal List<uint> BuffersToDelete = new List<uint>();
        internal List<uint> ProgramsToDelete = new List<uint>();
        internal List<uint> TexturesToDelete = new List<uint>();

        // list of Contexts and their associated Metadata
        private readonly Dictionary<Context, ContextMeta> contextMetadata = new Dictionary<Context, ContextMeta>();
        private readonly List<Context> disposedContexts = new List<Context>();

        // stored delegates for deleting graphics resources
        private delegate void DeleteResource(uint id);
        private readonly DeleteResource deleteArray = GL.DeleteVertexArray;
        private readonly DeleteResource deleteFramebuffer = GL.DeleteFramebuffer;
        private readonly DeleteResource deleteBuffer = GL.DeleteBuffer;
        private readonly DeleteResource deleteTexture = GL.DeleteTexture;
        private readonly DeleteResource deleteProgram = GL.DeleteProgram;

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

            // check for any resources we're still tracking that are tied to contexts
            {
                var currentContext = App.System.GetCurrentContext();

                lock (contextMetadata)
                {
                    foreach (var kv in contextMetadata)
                    {
                        var context = kv.Key;
                        var meta = kv.Value;
                        if (context.Disposed)
                        {
                            disposedContexts.Add(context);
                        }
                        else if (
                            (meta.FrameBuffersToDelete.Count > 0 || meta.VertexArraysToDelete.Count > 0) && 
                            (context.ActiveThreadId == 0 || context == currentContext))
                        {
                            lock (context)
                            {
                                context.MakeCurrent();

                                DeleteResources(deleteFramebuffer, meta.FrameBuffersToDelete);
                                DeleteResources(deleteArray, meta.VertexArraysToDelete);

                                currentContext?.MakeCurrent();
                            }
                        }
                    }

                    foreach (var context in disposedContexts)
                        contextMetadata.Remove(context);
                }
            }
        }

        protected override void AfterRender(WindowTarget target)
        {
            GL.Flush();
        }

        private void DeleteResources(DeleteResource deleter, List<uint> list)
        {
            lock (list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                    deleter(list[i]);
                list.Clear();
            }
        }

        internal ContextMeta GetContextMeta(Context context)
        {
            if (!contextMetadata.TryGetValue(context, out var meta))
                contextMetadata[context] = meta = new ContextMeta();
            return meta;
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

        protected override void PerformDraw(ref RenderPass state)
        {
            if (state.Target is GL_WindowTarget windowTarget)
            {
                lock (windowTarget.Window.Context)
                {
                    windowTarget.Window.Context.MakeCurrent();
                    Draw(ref state, windowTarget.Window.Context);
                }
            }
            else if (MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (BackgroundContext)
                {
                    BackgroundContext.MakeCurrent();
                    Draw(ref state, BackgroundContext);
                    GL.Flush();
                    BackgroundContext.MakeNonCurrent();
                }
            }
            else
            {
                var context = App.System.GetCurrentContext();
                if (context == null)
                    throw new Exception("Context is null");

                lock (context)
                {
                    Draw(ref state, context);
                }
            }

            void Draw(ref RenderPass state, Context context)
            {
                RenderPass lastState;

                // get the previous state
                var updateAll = false;
                var contextMeta = GetContextMeta(context);
                if (contextMeta.LastRenderState == null)
                {
                    updateAll = true;
                    lastState = state;
                }
                else
                    lastState = contextMeta.LastRenderState.Value;
                contextMeta.LastRenderState = state;

                // Bind the Target
                if (updateAll || lastState.Target != state.Target)
                {
                    if (state.Target is GL_WindowTarget)
                    {
                        GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
                    }
                    else if (state.Target is GL_RenderTexture glRenderTexture)
                    {
                        glRenderTexture.Bind(context);
                    }
                }

                // Use the Shader
                if (state.Material.Shader is GL_Shader glShader)
                    glShader.Use(state.Material);

                // Bind the Mesh
                if (state.Mesh is GL_Mesh glMesh)
                    glMesh.Bind(context, state.Material);

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
                            GL.CullFace(GLEnum.BACK);
                        else if (state.CullMode == Cull.Front)
                            GL.CullFace(GLEnum.FRONT);
                        else
                            GL.CullFace(GLEnum.FRONT_AND_BACK);
                    }
                }

                // Viewport
                if (updateAll || contextMeta.LastViewport == null || contextMeta.LastViewport.Value != state.Target.Viewport)
                {
                    GL.Viewport(state.Target.Viewport.X, state.Target.Viewport.Y, state.Target.Viewport.Width, state.Target.Viewport.Height);
                    contextMeta.LastViewport = state.Target.Viewport;
                }

                // Scissor
                if (updateAll || lastState.Scissor != state.Scissor || contextMeta.ForceScissorUpdate)
                {
                    if (state.Scissor.X <= 0 && state.Scissor.Y <= 0 && state.Scissor.Right >= state.Target.Viewport.Width && state.Scissor.Bottom >= state.Target.Viewport.Height)
                    {
                        GL.Disable(GLEnum.SCISSOR_TEST);
                    }
                    else
                    {
                        GL.Enable(GLEnum.SCISSOR_TEST);
                        GL.Scissor(state.Scissor.X, state.Target.Viewport.Height - state.Scissor.Bottom, state.Scissor.Width, state.Scissor.Height);
                    }
                }

                // Draw the Mesh
                {
                    if (state.MeshInstanceCount > 0)
                    {
                        GL.DrawElementsInstanced(GLEnum.TRIANGLES, state.MeshElementCount * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * state.MeshStartElement * 3), state.MeshInstanceCount);
                    }
                    else
                    {
                        GL.DrawElements(GLEnum.TRIANGLES, state.MeshElementCount * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * state.MeshStartElement * 3));
                    }

                    GL.BindVertexArray(0);
                }
            }
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
