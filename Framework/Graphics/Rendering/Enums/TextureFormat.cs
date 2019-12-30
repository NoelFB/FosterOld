using System;
namespace Foster.Framework
{
    public enum TextureFormat
    {
        /// <summary>
        /// No Texture Format
        /// </summary>
        None,

        /// <summary>
        /// ARGB Unsigned 32 Integer Color
        /// </summary>
        Color,

        /// <summary>
        /// Depth 24
        /// </summary>
        Depth24,

        /// <summary>
        /// Depth 24 Stencil 8
        /// </summary>
        Depth24Stencil8,
    }

    public static class TextureFormatExt
    {
        public static bool IsColorFormat(this TextureFormat format)
        {
            return format == TextureFormat.Color;
        }

        public static bool IsDepthStencilFormat(this TextureFormat format)
        {
            return format == TextureFormat.Depth24 ||
                   format == TextureFormat.Depth24Stencil8;
        }
    }
}
