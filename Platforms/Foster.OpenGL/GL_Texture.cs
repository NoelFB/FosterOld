using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.OpenGL
{
    public class GL_Texture : Texture
    {

        public readonly uint ID;
        private TextureFilter filter;
        private TextureWrap wrapX;
        private TextureWrap wrapY;

        public override TextureFilter Filter
        {
            get => filter;
            set
            {
                var f = (value == TextureFilter.Nearest ? GLEnum.NEAREST : GLEnum.LINEAR);

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
                var s = (value == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

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
                var t = (value == TextureWrap.Clamp ? GLEnum.CLAMP_TO_EDGE : GLEnum.REPEAT);

                GL.ActiveTexture((uint)GLEnum.TEXTURE0);
                GL.BindTexture(GLEnum.TEXTURE_2D, ID);
                GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, (int)t);

                wrapY = value;
            }
        }

        public GL_Texture(GL_Graphics graphics, int width, int height) : base(graphics)
        {
            unsafe
            {
                fixed (uint* id = &ID)
                    GL.GenTextures(1, new IntPtr(id));
            }

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, width, height, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(0));

            Width = width;
            Height = height;
            WrapX = TextureWrap.Wrap;
            WrapY = TextureWrap.Wrap;
            Filter = TextureFilter.Linear;

            GL.BindTexture(GLEnum.TEXTURE_2D, 0);
        }

        public override unsafe void GetData<T>(Memory<T> buffer)
        {
            using var handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.TexImage2D(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, Width, Height, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
            GL.BindTexture(GLEnum.TEXTURE_2D, 0);
        }

        public override unsafe void SetData<T>(Memory<T> buffer)
        {
            using var handle = buffer.Pin();

            GL.ActiveTexture((uint)GLEnum.TEXTURE0);
            GL.BindTexture(GLEnum.TEXTURE_2D, ID);
            GL.GetTexImage(GLEnum.TEXTURE_2D, 0, GLEnum.RGBA, GLEnum.UNSIGNED_BYTE, new IntPtr(handle.Pointer));
            GL.BindTexture(GLEnum.TEXTURE_2D, 0);
        }
    }
}
