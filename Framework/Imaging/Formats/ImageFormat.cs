using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    public abstract class ImageFormat
    {

        public abstract string Name { get; }
        public abstract bool IsFormat(Stream stream);
        public abstract Bitmap Read(Stream stream);
        public abstract void Write(Stream stream, Bitmap bitmap);

    }
}
