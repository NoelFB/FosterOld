using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    public static class Images
    {

        public delegate bool IsValidDelegate(Stream stream);
        public delegate bool ReadDelegate(Stream stream, out int width, out int height, out Color[]? pixels);
        public delegate bool WriteDelegate(Stream stream, int width, int height, Color[] pixels);

        public class Format
        {
            public readonly string Name;
            public readonly IsValidDelegate IsValid;
            public readonly ReadDelegate Read;
            public readonly WriteDelegate Write;

            public Format(string name, IsValidDelegate isValid, ReadDelegate read, WriteDelegate write)
            {
                Name = name;
                IsValid = isValid;
                Read = read;
                Write = write;
            }
        }

        public static readonly List<Format> Formats = new List<Format>();

        static Images()
        {
            Formats.Add(new Format("PNG", PNG.IsValid, PNG.Read, PNG.Write));
        }

        public static bool IsValid(Stream stream)
        {
            for (int i = 0; i < Formats.Count; i++)
                if (Formats[i].IsValid(stream))
                    return true;
            return false;
        }

        public static bool Read(Stream stream, out int width, out int height, out Color[]? pixels)
        {
            width = 0;
            height = 0;
            pixels = null;

            for (int i = 0; i < Formats.Count; i++)
            {
                if (Formats[i].IsValid(stream))
                {
                    return Formats[i].Read(stream, out width, out height, out pixels);
                }
            }

            return false;
        }

        public static bool Write(Stream stream, Format format, int width, int height, Color[] pixels)
        {
            return format.Write(stream, width, height, pixels);
        }

    }
}
