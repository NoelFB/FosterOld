using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_Graphics : Graphics
    {

        public event Action? OnResourceCleanup;

        protected override void OnCreated()
        {
            Api = GraphicsApi.OpenGL;
            ApiName = "OpenGL";
        }

        protected override void OnDisplayed()
        {
            GL.Init();
            GL.Enable(GLEnum.BLEND);

            MaxTextureSize = GL.MaxTextureSize;
            ApiVersion = new Version(GL.MajorVersion, GL.MinorVersion);

            base.OnDisplayed();
        }

        protected override void OnPostUpdate()
        {
            OnResourceCleanup?.Invoke();
            OnResourceCleanup = null;
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

        public override Texture CreateTexture(int width, int height)
        {
            return new GL_Texture(this, width, height);
        }

        public override Target CreateTarget(int width, int height, int textures = 1, bool depthBuffer = false)
        {
            return new GL_Target(this, width, height, textures, depthBuffer);
        }

        public override Shader CreateShader(string vertexSource, string fragmentSource)
        {
            return new GL_Shader(this, vertexSource, fragmentSource);
        }

        public override Mesh<T> CreateMesh<T>()
        {
            return new GL_Mesh<T>(this);
        }

        public override void Target(Target? target)
        {
            GL.BindFramebuffer(GLEnum.FRAMEBUFFER, (target as GL_Target)?.ID ?? 0);

            if (target != null)
            {
                Viewport = new RectInt(0, 0, target.Width, target.Height);
            }
            else if (App.System?.CurrentWindow != null && App.System.CurrentWindow.Opened)
            {
                Viewport = new RectInt(0, 0, App.System.CurrentWindow.DrawSize.X, App.System.CurrentWindow.DrawSize.Y);
            }
        }

        public override void DepthTest(bool enabled)
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

        public override void CullMode(Cull mode)
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

        public override void BlendMode(BlendMode blendMode)
        {
            GLEnum op = GetBlendFunc(blendMode.Operation);
            GLEnum src = GetBlendFactor(blendMode.Source);
            GLEnum dst = GetBlendFactor(blendMode.Destination);

            GL.BlendEquation(op);
            GL.BlendFunc(src, dst);
        }

        public override void Clear(Color color)
        {
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(GLEnum.COLOR_BUFFER_BIT);
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
