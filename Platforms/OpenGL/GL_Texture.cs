using Foster.Framework;
using System;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_Texture : Texture
    {

        public uint ID { get; private set; }

        private readonly GL_Graphics graphics;
        private readonly GLEnum glInternalFormat;
        private readonly GLEnum glFormat;
        private readonly GLEnum glType;
        private readonly bool flipVertically;
        
        public override bool FlipVertically => flipVertically;

        internal GL_Texture(GL_Graphics graphics, int width, int height, TextureFormat format, bool flipVertically) : base(width, height, format)
        {
            this.graphics = graphics;
            this.flipVertically = flipVertically;

            glInternalFormat = format switch
            {
                TextureFormat.Red => GLEnum.RED,
                TextureFormat.RG => GLEnum.RG,
                TextureFormat.RGB => GLEnum.RGB,
                TextureFormat.Color => GLEnum.RGBA,
                TextureFormat.DepthStencil => GLEnum.DEPTH24_STENCIL8,
                _ => throw new Exception("Invalid Texture Format"),
            };

            glFormat = format switch
            {
                TextureFormat.Red => GLEnum.RED,
                TextureFormat.RG => GLEnum.RG,
                TextureFormat.RGB => GLEnum.RGB,
                TextureFormat.Color => GLEnum.RGBA,
                TextureFormat.DepthStencil => GLEnum.DEPTH_STENCIL,
                _ => throw new Exception("Invalid Texture Format"),
            };

            glType = format switch
            {
                TextureFormat.Red => GLEnum.UNSIGNED_BYTE,
                TextureFormat.RG => GLEnum.UNSIGNED_BYTE,
                TextureFormat.RGB => GLEnum.UNSIGNED_BYTE,
                TextureFormat.Color => GLEnum.UNSIGNED_BYTE,
                TextureFormat.DepthStencil => GLEnum.UNSIGNED_INT_24_8,
                _ => throw new Exception("Invalid Texture Format"),
            };

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock(graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    Init();
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);
                }
            }
            else
            {
                Init();
            }

            void Init()
            {
                ID = GL.GenTexture();
                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);

                GL.TexImage2D(GLEnum.TEXTURE_2D, 0, glInternalFormat, width, height, 0, glFormat, glType, new IntPtr(0));
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)(Filter == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR));
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)(Filter == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR));
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)(WrapX == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT));
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)(WrapY == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT));
            }
        }

        ~GL_Texture()
        {
            Dispose();
        }

        protected override void SetFilter(TextureFilter filter)
        {
            GLEnum f = (filter == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR);

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    SetFilter(ID, f);
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);
                }
            }
            else
            {
                SetFilter(ID, f);
            }

            static void SetFilter(uint id, GLEnum f)
            {
                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, id);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)f);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)f);
            }
        }

        protected override void SetWrap(TextureWrap x, TextureWrap y)
        {
            GLEnum s = (x == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);
            GLEnum t = (y == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    SetFilter(ID, s, t);
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);
                }
            }
            else
            {
                SetFilter(ID, s, t);
            }

            static void SetFilter(uint id, GLEnum s, GLEnum t)
            {
                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, id);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)s);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)t);
            }
        }

        protected override unsafe void SetDataInternal<T>(ReadOnlyMemory<T> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    Upload();
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);
                }
            }
            else
            {
                Upload();
            }

            void Upload()
            {
                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.TexImage2D(GLEnum.TEXTURE_2D, 0, glInternalFormat, Width, Height, 0, glFormat, glType, new IntPtr(handle.Pointer));
            }
        }

        protected override unsafe void GetDataInternal<T>(Memory<T> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    Download();
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);
                }
            }
            else
            {
                Download();
            }

            void Download()
            {
                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.GetTexImage(GLEnum.TEXTURE_2D, 0, glInternalFormat, glType, new IntPtr(handle.Pointer));
            }
        }

        public override void Dispose()
        {
            if (ID != 0)
            {
                graphics.TexturesToDelete.Add(ID);
                ID = 0;
            }
        }
    }
}
