using System.Collections.Generic;
using System.IO;

namespace Foster.Framework
{
    /// <summary>
    /// An Abstract Image Format
    /// </summary>
    public abstract class ImageFormat
    {

        public readonly string Name;

        protected ImageFormat(string name)
        {
            Name = name;
        }

        public abstract bool IsValid(Stream stream);
        public abstract bool Read(Stream stream, out int width, out int height, out Color[] pixels);
        public abstract bool Write(Stream stream, int width, int height, Color[] pixels);

        public static ImageFormat Png = new PngFormat();

        public static List<ImageFormat> Formats = new List<ImageFormat>()
        {
            Png
        };

    }
}
