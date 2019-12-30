using Foster.Framework;
using Foster.Framework.Internal;
using System;

namespace Foster.OpenGL
{
    public class GL_Texture : InternalTexture
    {

        public uint ID { get; private set; }

        private readonly GL_Graphics graphics;
        private readonly int width;
        private readonly int height;

        internal readonly TextureFormat format;
        internal readonly GLEnum glFormat;
        internal bool flipVertically;
        
        public override bool FlipVertically => flipVertically;

        internal GL_Texture(GL_Graphics graphics, int width, int height, TextureFormat format)
        {
            this.graphics = graphics;
            this.width = width;
            this.height = height;
            this.format = format;

            ID = GL.GenTexture();
            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);

            switch (format)
            {
                case TextureFormat.Color:
                    glFormat = GLEnum.RGBA;
                    break;
                case TextureFormat.Depth24:
                    glFormat = GLEnum.DEPTH_COMPONENT24;
                    break;
                case TextureFormat.Depth24Stencil8:
                    glFormat = GLEnum.DEPTH24_STENCIL8;
                    break;
                default:
                    throw new Exception("Invalid Texture Format");
            }

            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, glFormat, width, height, 0, glFormat, GLEnum.UNSIGNED_BYTE, new IntPtr(0));        
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)GLEnum.LINEAR);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)GLEnum.LINEAR);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)GLEnum.REPEAT);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)GLEnum.REPEAT);
        }

        ~GL_Texture()
        {
            DisposeResources();
        }

        protected override void SetFilter(TextureFilter filter)
        {
            GLEnum f = (filter == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR);

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)f);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)f);
        }

        protected override void SetWrap(TextureWrap x, TextureWrap y)
        {
            GLEnum s = (x == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);
            GLEnum t = (y == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)s);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)t);
        }

        protected override unsafe void SetData<T>(Memory<T> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, glFormat, width, height, 0, glFormat, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
        }

        protected override unsafe void GetData<T>(Memory<T> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.GetTexImage(GLEnum.TEXTURE_2D, 0, glFormat, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
        }

        protected override void DisposeResources()
        {
            if (ID != 0)
            {
                graphics.TexturesToDelete.Add(ID);
                ID = 0;
            }
        }
    }
}
