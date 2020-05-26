using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics, IGraphicsOpenGL
    {
        internal ISystemOpenGL System => App.System as ISystemOpenGL ?? throw new Exception("System does not implement IGLSystem");

        // Background Context can be null up until Startup, at which point they never are again
        internal ISystemOpenGL.Context BackgroundContext = null!;

        // Stores info about the Context
        internal class ContextMeta
        {
            public List<uint> VertexArraysToDelete = new List<uint>();
            public List<uint> FrameBuffersToDelete = new List<uint>();
            public RenderTarget? LastRenderTarget;
            public RenderPass? LastRenderState;
            public RectInt Viewport;
            public bool ForceScissorUpdate;
        }

        // various resources waiting to be deleted
        internal List<uint> BuffersToDelete = new List<uint>();
        internal List<uint> ProgramsToDelete = new List<uint>();
        internal List<uint> TexturesToDelete = new List<uint>();

        // list of Contexts and their associated Metadata
        private readonly Dictionary<ISystemOpenGL.Context, ContextMeta> contextMetadata = new Dictionary<ISystemOpenGL.Context, ContextMeta>();
        private readonly List<ISystemOpenGL.Context> disposedContexts = new List<ISystemOpenGL.Context>();

        // stored delegates for deleting graphics resources
        private delegate void DeleteResource(uint id);
        private readonly DeleteResource deleteArray = GL.DeleteVertexArray;
        private readonly DeleteResource deleteFramebuffer = GL.DeleteFramebuffer;
        private readonly DeleteResource deleteBuffer = GL.DeleteBuffer;
        private readonly DeleteResource deleteTexture = GL.DeleteTexture;
        private readonly DeleteResource deleteProgram = GL.DeleteProgram;

        public override Renderer Renderer => Renderer.OpenGL;

        protected override void ApplicationStarted()
        {
            ApiName = "OpenGL";
        }

        protected override void FirstWindowCreated()
        {
            GL.Init(this, System);
            GL.DepthMask(true);

            MaxTextureSize = GL.MaxTextureSize;
            OriginBottomLeft = true;
            ApiVersion = new Version(GL.MajorVersion, GL.MinorVersion);
            DeviceName = GL.GetString(GLEnum.RENDERER);

            BackgroundContext = System.CreateGLContext();
        }

        protected override void Shutdown()
        {
            BackgroundContext.Dispose();
        }

        protected override void FrameStart()
        {
            // delete any GL graphics resources that are shared between contexts
            DeleteResources(deleteBuffer, BuffersToDelete);
            DeleteResources(deleteProgram, ProgramsToDelete);
            DeleteResources(deleteTexture, TexturesToDelete);

            // check for any resources we're still tracking that are tied to contexts
            {
                var lastContext = System.GetCurrentGLContext();
                var changedContext = false;

                lock (contextMetadata)
                {
                    foreach (var kv in contextMetadata)
                    {
                        var context = kv.Key;
                        var meta = kv.Value;

                        if (context.IsDisposed)
                        {
                            disposedContexts.Add(context);
                        }
                        else if (meta.FrameBuffersToDelete.Count > 0 || meta.VertexArraysToDelete.Count > 0)
                        {
                            lock (context)
                            {
                                System.SetCurrentGLContext(context);

                                DeleteResources(deleteFramebuffer, meta.FrameBuffersToDelete);
                                DeleteResources(deleteArray, meta.VertexArraysToDelete);

                                changedContext = true;
                            }
                        }
                    }

                    foreach (var context in disposedContexts)
                        contextMetadata.Remove(context);
                }

                if (changedContext)
                    System.SetCurrentGLContext(lastContext);
            }
        }

        protected override void AfterRenderWindow(Window window)
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

        internal ContextMeta GetContextMeta(ISystemOpenGL.Context context)
        {
            if (!contextMetadata.TryGetValue(context, out var meta))
                contextMetadata[context] = meta = new ContextMeta();
            return meta;
        }

        protected override Texture.Platform CreateTexture(int width, int height, TextureFormat format)
        {
            return new GL_Texture(this);
        }

        protected override FrameBuffer.Platform CreateFrameBuffer(int width, int height, TextureFormat[] attachments)
        {
            return new GL_FrameBuffer(this, width, height, attachments);
        }

        protected override Shader.Platform CreateShader(ShaderSource source)
        {
            return new GL_Shader(this, source);
        }

        protected override Mesh.Platform CreateMesh()
        {
            return new GL_Mesh(this);
        }

        protected override ShaderSource CreateShaderSourceBatch2D()
        {
            var vertexSource = Calc.EmbeddedResourceText("Resources/batch2d.vert");
            var fragmentSource = Calc.EmbeddedResourceText("Resources/batch2d.frag");

            return new ShaderSource(vertexSource, fragmentSource);
        }

        protected override void ClearInternal(RenderTarget target, Clear flags, Color color, float depth, int stencil, RectInt viewport)
        {
            if (target is Window window)
            {
                var context = System.GetWindowGLContext(window);
                lock (context)
                {
                    if (context != System.GetCurrentGLContext())
                        System.SetCurrentGLContext(context);
                    GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
                    Clear(this, context, target, flags, color, depth, stencil, viewport);
                }
            }
            else if (target is FrameBuffer rt && rt.Implementation is GL_FrameBuffer renderTexture)
            {
                // if we're off the main thread, clear using the Background Context
                if (MainThreadId != Thread.CurrentThread.ManagedThreadId)
                {
                    lock (BackgroundContext)
                    {
                        System.SetCurrentGLContext(BackgroundContext);

                        renderTexture.Bind(BackgroundContext);
                        Clear(this, BackgroundContext, target, flags, color, depth, stencil, viewport);
                        GL.Flush();

                        System.SetCurrentGLContext(null);
                    }
                }
                // otherwise just clear, regardless of Context
                else
                {
                    var context = System.GetCurrentGLContext();
                    if (context == null)
                    {
                        context = System.GetWindowGLContext(App.Window);
                        System.SetCurrentGLContext(context);
                    }

                    lock (context)
                    {
                        renderTexture.Bind(context);
                        Clear(this, context, target, flags, color, depth, stencil, viewport);
                    }
                }
            }

            static void Clear(GL_Graphics graphics, ISystemOpenGL.Context context, RenderTarget target, Clear flags, Color color, float depth, int stencil, RectInt viewport)
            {
                // update the viewport
                var meta = graphics.GetContextMeta(context);
                {
                    viewport.Y = target.RenderHeight - viewport.Y - viewport.Height;

                    if (meta.Viewport != viewport)
                    {
                        GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
                        meta.Viewport = viewport;
                    }
                }

                // we disable the scissor for clearing
                meta.ForceScissorUpdate = true;
                GL.Disable(GLEnum.SCISSOR_TEST);

                // clear
                var mask = GLEnum.ZERO;

                if (flags.HasFlag(Framework.Clear.Color))
                {
                    GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                    mask |= GLEnum.COLOR_BUFFER_BIT;
                }

                if (flags.HasFlag(Framework.Clear.Depth))
                {
                    GL.ClearDepth(depth);
                    mask |= GLEnum.DEPTH_BUFFER_BIT;
                }

                if (flags.HasFlag(Framework.Clear.Stencil))
                {
                    GL.ClearStencil(stencil);
                    mask |= GLEnum.STENCIL_BUFFER_BIT;
                }

                GL.Clear(mask);
                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
            }
        }

        protected override void RenderInternal(ref RenderPass pass)
        {
            if (pass.Target is Window window)
            {
                var context = System.GetWindowGLContext(window);
                lock (context)
                {
                    if (context != System.GetCurrentGLContext())
                        System.SetCurrentGLContext(context);
                    Draw(this, ref pass, context);
                }
            }
            else if (MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (BackgroundContext)
                {
                    System.SetCurrentGLContext(BackgroundContext);

                    Draw(this, ref pass, BackgroundContext);
                    GL.Flush();

                    System.SetCurrentGLContext(null);
                }
            }
            else
            {
                var context = System.GetCurrentGLContext();
                if (context == null)
                {
                    context = System.GetWindowGLContext(App.Window);
                    System.SetCurrentGLContext(context);
                }

                lock (context)
                {
                    Draw(this, ref pass, context);
                }
            }

            static void Draw(GL_Graphics graphics, ref RenderPass pass, ISystemOpenGL.Context context)
            {
                RenderPass lastPass;

                // get the previous state
                var updateAll = false;
                var contextMeta = graphics.GetContextMeta(context);
                if (contextMeta.LastRenderState == null)
                {
                    updateAll = true;
                    lastPass = pass;
                }
                else
                    lastPass = contextMeta.LastRenderState.Value;
                contextMeta.LastRenderState = pass;

                // Bind the Target
                if (updateAll || contextMeta.LastRenderTarget != pass.Target)
                {
                    if (pass.Target is Window)
                    {
                        GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
                    }
                    else if (pass.Target is FrameBuffer rt && rt.Implementation is GL_FrameBuffer renderTexture)
                    {
                        renderTexture.Bind(context);
                    }

                    contextMeta.LastRenderTarget = pass.Target;
                }

                // Use the Shader
                if (pass.Material.Shader.Implementation is GL_Shader glShader)
                    glShader.Use(pass.Material);

                // Bind the Mesh
                if (pass.Mesh.Implementation is GL_Mesh glMesh)
                    glMesh.Bind(context, pass.Material);

                // Blend Mode
                {
                    GL.Enable(GLEnum.BLEND);

                    if (updateAll ||
                        lastPass.BlendMode.ColorOperation != pass.BlendMode.ColorOperation ||
                        lastPass.BlendMode.AlphaOperation != pass.BlendMode.AlphaOperation)
                    {
                        GLEnum colorOp = GetBlendFunc(pass.BlendMode.ColorOperation);
                        GLEnum alphaOp = GetBlendFunc(pass.BlendMode.AlphaOperation);

                        GL.BlendEquationSeparate(colorOp, alphaOp);
                    }

                    if (updateAll ||
                        lastPass.BlendMode.ColorSource != pass.BlendMode.ColorSource ||
                        lastPass.BlendMode.ColorDestination != pass.BlendMode.ColorDestination ||
                        lastPass.BlendMode.AlphaSource != pass.BlendMode.AlphaSource ||
                        lastPass.BlendMode.AlphaDestination != pass.BlendMode.AlphaDestination)
                    {
                        GLEnum colorSrc = GetBlendFactor(pass.BlendMode.ColorSource);
                        GLEnum colorDst = GetBlendFactor(pass.BlendMode.ColorDestination);
                        GLEnum alphaSrc = GetBlendFactor(pass.BlendMode.AlphaSource);
                        GLEnum alphaDst = GetBlendFactor(pass.BlendMode.AlphaDestination);

                        GL.BlendFuncSeparate(colorSrc, colorDst, alphaSrc, alphaDst);
                    }

                    if (updateAll || lastPass.BlendMode.Mask != pass.BlendMode.Mask)
                    {
                        GL.ColorMask(
                            (pass.BlendMode.Mask & BlendMask.Red) != 0,
                            (pass.BlendMode.Mask & BlendMask.Green) != 0,
                            (pass.BlendMode.Mask & BlendMask.Blue) != 0,
                            (pass.BlendMode.Mask & BlendMask.Alpha) != 0);
                    }

                    if (updateAll || lastPass.BlendMode.Color != pass.BlendMode.Color)
                    {
                        GL.BlendColor(
                            pass.BlendMode.Color.R / 255f,
                            pass.BlendMode.Color.G / 255f,
                            pass.BlendMode.Color.B / 255f,
                            pass.BlendMode.Color.A / 255f);
                    }
                }

                // Depth Function
                if (updateAll || lastPass.DepthFunction != pass.DepthFunction)
                {
                    if (pass.DepthFunction == Compare.None)
                    {
                        GL.Disable(GLEnum.DEPTH_TEST);
                    }
                    else
                    {
                        GL.Enable(GLEnum.DEPTH_TEST);

                        switch (pass.DepthFunction)
                        {
                            case Compare.Always:
                                GL.DepthFunc(GLEnum.ALWAYS);
                                break;
                            case Compare.Equal:
                                GL.DepthFunc(GLEnum.EQUAL);
                                break;
                            case Compare.Greater:
                                GL.DepthFunc(GLEnum.GREATER);
                                break;
                            case Compare.GreaterOrEqual:
                                GL.DepthFunc(GLEnum.GEQUAL);
                                break;
                            case Compare.Less:
                                GL.DepthFunc(GLEnum.LESS);
                                break;
                            case Compare.LessOrEqual:
                                GL.DepthFunc(GLEnum.LEQUAL);
                                break;
                            case Compare.Never:
                                GL.DepthFunc(GLEnum.NEVER);
                                break;
                            case Compare.NotEqual:
                                GL.DepthFunc(GLEnum.NOTEQUAL);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }

                // Cull Mode
                if (updateAll || lastPass.CullMode != pass.CullMode)
                {
                    if (pass.CullMode == CullMode.None)
                    {
                        GL.Disable(GLEnum.CULL_FACE);
                    }
                    else
                    {
                        GL.Enable(GLEnum.CULL_FACE);

                        if (pass.CullMode == CullMode.Back)
                            GL.CullFace(GLEnum.BACK);
                        else if (pass.CullMode == CullMode.Front)
                            GL.CullFace(GLEnum.FRONT);
                        else
                            GL.CullFace(GLEnum.FRONT_AND_BACK);
                    }
                }

                // Viewport
                var viewport = pass.Viewport ?? new RectInt(0, 0, pass.Target.RenderWidth, pass.Target.RenderHeight);
                {
                    viewport.Y = pass.Target.RenderHeight - viewport.Y - viewport.Height;

                    if (updateAll || contextMeta.Viewport != viewport)
                    {
                        GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
                        contextMeta.Viewport = viewport;
                    }
                }

                // Scissor
                {
                    var scissor = pass.Scissor ?? new RectInt(0, 0, pass.Target.RenderWidth, pass.Target.RenderHeight);
                    scissor.Y = pass.Target.RenderHeight - scissor.Y - scissor.Height;
                    scissor.Width = Math.Max(0, scissor.Width);
                    scissor.Height = Math.Max(0, scissor.Height);

                    if (updateAll || lastPass.Scissor != scissor || contextMeta.ForceScissorUpdate)
                    {
                        if (pass.Scissor == null)
                        {
                            GL.Disable(GLEnum.SCISSOR_TEST);
                        }
                        else
                        {
                            GL.Enable(GLEnum.SCISSOR_TEST);
                            GL.Scissor(scissor.X, scissor.Y, scissor.Width, scissor.Height);
                        }

                        contextMeta.ForceScissorUpdate = false;
                        lastPass.Scissor = scissor;
                    }
                }
                GLEnum indexType = pass.IndexElementSize == IndexElementSize.ThirtyTwoBits? GLEnum.UNSIGNED_INT: GLEnum.UNSIGNED_SHORT;
                int indexSize = pass.IndexElementSize == IndexElementSize.ThirtyTwoBits ? sizeof(int) : sizeof(ushort);

                // Draw the Mesh
                {
                    if (pass.MeshInstanceCount > 0)
                    {
                        GL.DrawElementsInstanced(GLEnum.TRIANGLES, (int)(pass.MeshIndexCount), indexType, new IntPtr(indexSize * pass.MeshIndexStart), (int)pass.MeshInstanceCount);
                    }
                    else
                    {
                        GL.DrawElements(GLEnum.TRIANGLES, (int)(pass.MeshIndexCount), indexType, new IntPtr(indexSize * pass.MeshIndexStart));
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
