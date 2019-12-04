using Foster.Framework;
using System;

namespace Foster.OpenGL
{
    public class GL_Texture : Texture
    {

        public uint ID { get; private set; }
        private TextureFilter filter;
        private TextureWrap wrapX;
        private TextureWrap wrapY;
        internal bool flipVertically;

        public override TextureFilter Filter
        {
            get => filter;
            set
            {
                GLEnum f = (value == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR);

                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)f);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)f);

                filter = value;
            }
        }

        public override TextureWrap WrapX
        {
            get => wrapX;
            set
            {
                GLEnum s = (value == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)s);

                wrapX = value;
            }
        }

        public override TextureWrap WrapY
        {
            get => wrapY;
            set
            {
                GLEnum t = (value == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)t);

                wrapY = value;
            }
        }

        public override bool FlipVertically => flipVertically;

        public GL_Texture(GL_Graphics graphics, int width, int height) : base(graphics)
        {
            ID = GL.GenTexture();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);

            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, width, height, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(0));
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)GLEnum.LINEAR);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)GLEnum.LINEAR);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, (int)GLEnum.REPEAT);
            GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)GLEnum.REPEAT);

            Width = width;
            Height = height;
        }

        public override unsafe void SetColor(Memory<Color> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, Width, Height, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
        }

        public override unsafe void GetColor(Memory<Color> buffer)
        {
            using System.Buffers.MemoryHandle handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.GetTexImage(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
        }

        public override void Dispose()
        {
            if (!Disposed && (Graphics is GL_Graphics graphics))
                graphics.TexturesToDelete.Add(ID);

            base.Dispose();
        }
    }
}
