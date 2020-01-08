using System;
namespace Foster.Framework
{
    /// <summary>
    /// Texture Format
    /// </summary>
    public enum TextureFormat
    {
        /// <summary>
        /// No Texture Format
        /// </summary>
        None,

        /// <summary>
        /// Single 8-bit component stored in the Red channel
        /// </summary>
        Red,

        /// <summary>
        /// Two 8-bit components stored in the Red, Green channels
        /// </summary>
        RG,

        /// <summary>
        /// Three 8-bit components stored in the Red, Green, Blue channels
        /// </summary>
        RGB,

        /// <summary>
        /// ARGB uint32 Color
        /// </summary>
        Color,

        /// <summary>
        /// Depth 24-bit Stencil 8-bit
        /// </summary>
        DepthStencil,
    }

    public static class TextureFormatExt
    {
        public static bool IsTextureColorFormat(this TextureFormat format)
        {
            return
                format == TextureFormat.Color ||
                format == TextureFormat.Red ||
                format == TextureFormat.RG ||
                format == TextureFormat.RGB;
        }

        public static bool IsDepthStencilFormat(this TextureFormat format)
        {
            return format == TextureFormat.DepthStencil;
        }
    }
}
