using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// This is not a true or full PNG format implementation, but rather handles the most common PNG formats for games
    /// It could probably be optimized more.
    /// </summary>
    public static class PNG
    {
        private enum Colors
        {
            Greyscale = 0,
            Truecolor = 2,
            Indexed = 3,
            GreyscaleAlpha = 4,
            TruecolorAlpha = 6
        }

        private enum Interlace
        {
            None = 0,
            Adam7 = 1
        }

        private static readonly byte[] header = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private static readonly uint[] crcTable = new uint[256];

        static PNG()
        {
            // create the CRC table
            // taken from libpng format specification: http://www.libpng.org/pub/png/spec/1.2/PNG-CRCAppendix.html

            for (int n = 0; n < 256; n++)
            {
                uint c = (uint)n;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & 1) != 0)
                        c = 0xedb88320U ^ (c >> 1);
                    else
                        c >>= 1;
                }
                crcTable[n] = c;
            }
        }

        public static bool IsValid(Stream stream)
        {
            var pos = stream.Position;

            // check PNG header
            bool isPng =
                stream.ReadByte() == header[0] && // 8-bit format
                stream.ReadByte() == header[1] && // P
                stream.ReadByte() == header[2] && // N
                stream.ReadByte() == header[3] && // G
                stream.ReadByte() == header[4] && // Carriage Return
                stream.ReadByte() == header[5] && // Line Feed
                stream.ReadByte() == header[6] && // Ctrl-Z
                stream.ReadByte() == header[7];   // Line Feed

            stream.Seek(pos, SeekOrigin.Begin);

            return isPng;
        }

        public static unsafe bool Read(Stream stream, out int width, out int height, out Color[] pixels)
        {

            // This could likely be optimized a buuunch more
            // We also ignore all checksums when reading because they don't seem super important for game usage

            width = height = 0;

            var hasTransparency = false;
            var depth = 8;
            var color = Colors.Truecolor;
            var compression = 0;
            var filter = 0;
            var interlace = Interlace.None;
            var components = 4;

            using MemoryStream idat = new MemoryStream();
            Span<byte> idatChunk = stackalloc byte[4096];
            Span<byte> palette = stackalloc byte[0];
            Span<byte> alphaPalette = stackalloc byte[0];
            Span<byte> fourbytes = stackalloc byte[4];

            bool hasIHDR = false, hasPLTE = false, hasIDAT = false;

            // Check PNG Header
            if (!IsValid(stream))
                throw new Exception("Stream is not PNG");

            // Skip PNG header
            stream.Seek(8, SeekOrigin.Current);

            // Read Chunks
            while (stream.Position < stream.Length)
            {
                long chunkStartPosition = stream.Position;

                // chunk length
                stream.Read(fourbytes);
                int chunkLength = SwapEndian(BitConverter.ToInt32(fourbytes));

                // chunk type
                stream.Read(fourbytes);

                // IHDR Chunk
                if (Check("IHDR", fourbytes))
                {
                    hasIHDR = true;
                    stream.Read(fourbytes);
                    width = SwapEndian(BitConverter.ToInt32(fourbytes));
                    stream.Read(fourbytes);
                    height = SwapEndian(BitConverter.ToInt32(fourbytes));
                    depth = stream.ReadByte();
                    color = (Colors)stream.ReadByte();
                    compression = stream.ReadByte();
                    filter = stream.ReadByte();
                    interlace = (Interlace)stream.ReadByte();
                    hasTransparency = color == Colors.GreyscaleAlpha || color == Colors.TruecolorAlpha;

                    if (color == Colors.Greyscale || color == Colors.Indexed)
                        components = 1;
                    else if (color == Colors.GreyscaleAlpha)
                        components = 2;
                    else if (color == Colors.Truecolor)
                        components = 3;
                    else if (color == Colors.TruecolorAlpha)
                        components = 4;

                    // currently don't support interlacing as I'm actually not sure where the interlace step takes place lol
                    if (interlace == Interlace.Adam7)
                        throw new NotImplementedException("Interlaced PNGs not implemented");

                    if (depth != 1 && depth != 2 && depth != 4 && depth != 8 && depth != 16)
                        throw new NotSupportedException($"{depth}-bit depth not supported");

                    if (filter != 0)
                        throw new NotSupportedException($"filter {filter} not supported");

                    if (compression != 0)
                        throw new NotSupportedException($"compression {compression} not supported");
                }
                // PLTE Chunk (Indexed Palette)
                else if (Check("PLTE", fourbytes))
                {
                    hasPLTE = true;
                    palette = stackalloc byte[chunkLength];
                    stream.Read(palette);
                }
                // IDAT Chunk (Image Data)
                else if (Check("IDAT", fourbytes))
                {
                    hasIDAT = true;

                    for (int i = 0; i < chunkLength; i += idatChunk.Length)
                    {
                        int size = Math.Min(idatChunk.Length, chunkLength - i);
                        stream.Read(idatChunk.Slice(0, size));
                        idat.Write(idatChunk.Slice(0, size));
                    }
                }
                // tRNS Chunk (Alpha Palette)
                else if (Check("tRNS", fourbytes))
                {
                    if (color == Colors.Indexed)
                    {
                        alphaPalette = stackalloc byte[chunkLength];
                        stream.Read(alphaPalette);
                    }
                    else if (color == Colors.Greyscale)
                    {

                    }
                    else if (color == Colors.Truecolor)
                    {

                    }
                }
                // bKGD Chunk (Background)
                else if (Check("bKGD", fourbytes))
                {

                }

                // seek to end of the chunk
                stream.Seek(chunkStartPosition + chunkLength + 12, SeekOrigin.Begin);
            }

            // checks
            if (!hasIHDR)
                throw new Exception("PNG Missing IHDR data");

            if (!hasIDAT)
                throw new Exception("PNG Missing IDAT data");

            if (!hasPLTE && color == Colors.Indexed)
                throw new Exception("PNG Missing PLTE data");

            // Parse the IDAT data into Pixels
            // It would be cool to do this line-by-line so we don't need to create a buffer to store the decompressed stream
            {
                byte[] buffer = new byte[width * height * (depth == 16 ? 2 : 1) * 4 + height];

                // decompress the image data
                // using deflate since it's built into C# and I ~think~ PNG always requires zlib to use deflate
                {
                    idat.Seek(2, SeekOrigin.Begin);
                    using DeflateStream deflateStream = new DeflateStream(idat, CompressionMode.Decompress);
                    deflateStream.Read(buffer);
                }

                // apply filter pass - this happens in-place
                {
                    int lineLength = (int)Math.Ceiling((width * components * depth) / 8f);
                    int bpp = Math.Max(1, (components * depth) / 8);
                    int dest = 0;

                    // each scanline
                    for (int y = 0; y < height; y++)
                    {
                        int source = y * (lineLength + 1) + 1;
                        byte lineFilter = buffer[source - 1];

                        // 0 - None
                        if (lineFilter == 0)
                        {
                            Array.Copy(buffer, source, buffer, dest, lineLength);
                        }
                        // 1 - Sub
                        else if (lineFilter == 1)
                        {
                            Array.Copy(buffer, source, buffer, dest, Math.Min(bpp, lineLength));
                            for (int x = bpp; x < lineLength; x++)
                            {
                                buffer[dest + x] = (byte)(buffer[source + x] + buffer[dest + x - bpp]);
                            }
                        }
                        // 2 - Up
                        else if (lineFilter == 2)
                        {
                            if (y <= 0)
                            {
                                Array.Copy(buffer, source, buffer, dest, lineLength);
                            }
                            else
                            {
                                for (int x = 0; x < lineLength; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + buffer[dest + x - lineLength]);
                                }
                            }
                        }
                        // 3 - Average
                        else if (lineFilter == 3)
                        {
                            if (y <= 0)
                            {
                                Array.Copy(buffer, source, buffer, dest, Math.Min(bpp, lineLength));
                                for (int x = bpp; x < lineLength; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + ((buffer[dest + x - bpp] + 0) / 2));
                                }
                            }
                            else
                            {
                                for (int x = 0; x < bpp; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + ((0 + buffer[dest + x - lineLength]) / 2));
                                }

                                for (int x = bpp; x < lineLength; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + ((buffer[dest + x - bpp] + buffer[dest + x - lineLength]) / 2));
                                }
                            }
                        }
                        // 4 - Paeth
                        else if (lineFilter == 4)
                        {
                            if (y <= 0)
                            {
                                Array.Copy(buffer, source, buffer, dest, Math.Min(bpp, lineLength));
                                for (int x = bpp; x < lineLength; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + buffer[dest + x - bpp]);
                                }
                            }
                            else
                            {
                                for (int x = 0, c = Math.Min(bpp, lineLength); x < c; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + buffer[dest + x - lineLength]);
                                }

                                for (int x = bpp; x < lineLength; x++)
                                {
                                    buffer[dest + x] = (byte)(buffer[source + x] + PaethPredictor(buffer[dest + x - bpp], buffer[dest + x - lineLength], buffer[dest + x - bpp - lineLength]));
                                }
                            }
                        }

                        dest += lineLength;
                    }
                }

                // if the bit-depth isn't 8, convert it
                if (depth != 8)
                {
                    throw new NotImplementedException("Non 8-bit PNGs not Implemented");
                }

                // Convert bytes to RGBA data
                // We work backwards as to not overwrite data
                {
                    // Indexed Color
                    if (color == Colors.Indexed)
                    {
                        for (int p = width * height - 1, i = width * height * 4 - 4; p >= 0; p--, i -= 4)
                        {
                            int id = buffer[p] * 3;
                            buffer[i + 3] = (alphaPalette == null || buffer[p] >= alphaPalette.Length) ? (byte)255 : alphaPalette[buffer[p]];
                            buffer[i + 2] = palette[id + 2];
                            buffer[i + 1] = palette[id + 1];
                            buffer[i + 0] = palette[id + 0];
                        }
                    }
                    // Grayscale
                    else if (color == Colors.Greyscale)
                    {
                        for (int p = width * height - 1, i = width * height * 4 - 4; p >= 0; p--, i -= 4)
                        {
                            buffer[i + 3] = 255;
                            buffer[i + 2] = buffer[p];
                            buffer[i + 1] = buffer[p];
                            buffer[i + 0] = buffer[p];
                        }
                    }
                    // Grayscale-Alpha
                    else if (color == Colors.GreyscaleAlpha)
                    {
                        for (int p = width * height * 2 - 2, i = width * height * 4 - 4; p >= 0; p -= 2, i -= 4)
                        {
                            byte val = buffer[p], alpha = buffer[p + 1];
                            buffer[i + 3] = buffer[p + 1];
                            buffer[i + 2] = buffer[p];
                            buffer[i + 1] = buffer[p];
                            buffer[i + 0] = buffer[p];
                        }
                    }
                    // Truecolor
                    else if (color == Colors.Truecolor)
                    {
                        for (int p = width * height * 3 - 3, i = width * height * 4 - 4; p >= 0; p -= 3, i -= 4)
                        {
                            buffer[i + 3] = 255;
                            buffer[i + 2] = buffer[p + 2];
                            buffer[i + 1] = buffer[p + 1];
                            buffer[i + 0] = buffer[p + 0];
                        }
                    }
                }

                // set RGBA data to Color array
                {
                    pixels = new Color[width * height];

                    fixed (byte* inArray = buffer)
                    fixed (Color* outArray = pixels)
                    {
                        Buffer.MemoryCopy(inArray, outArray, width * height * 4, width * height * 4);
                    }
                }
            }

            return true;
        }

        public static unsafe bool Write(Stream stream, int width, int height, Color[] pixels)
        {
            const int MaxIDATChunkLength = 8192;

            static void Chunk(BinaryWriter writer, string title, Span<byte> buffer)
            {
                // write chunk
                {
                    writer.Write(SwapEndian(buffer.Length));
                    for (int i = 0; i < title.Length; i++)
                        writer.Write((byte)title[i]);
                    writer.Write(buffer);
                }

                // write CRC
                {
                    uint crc = 0xFFFFFFFFU;
                    for (int n = 0; n < title.Length; n++)
                        crc = crcTable[(crc ^ (byte)title[n]) & 0xFF] ^ (crc >> 8);

                    for (int n = 0; n < buffer.Length; n++)
                        crc = crcTable[(crc ^ buffer[n]) & 0xFF] ^ (crc >> 8);

                    writer.Write(SwapEndian((int)(crc ^ 0xFFFFFFFFU)));
                }
            }

            static void WriteIDAT(BinaryWriter writer, MemoryStream memory, bool writeAll)
            {
                var zlib = new Span<byte>(memory.GetBuffer());
                var remainder = (int)memory.Position;
                var offset = 0;

                // write out IDAT chunks while there is memory to write
                while ((writeAll ? remainder > 0 : remainder >= MaxIDATChunkLength))
                {
                    var amount = Math.Min(remainder, MaxIDATChunkLength);

                    Chunk(writer, "IDAT", zlib.Slice(offset, amount));
                    offset += amount;
                    remainder -= amount;
                }

                // shift remaining memory back
                if (!writeAll)
                {
                    if (remainder > 0)
                        zlib.Slice(offset).CopyTo(zlib);
                    memory.Position = remainder;
                    memory.SetLength(remainder);
                }
            }

            // PNG header
            using BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(header);

            // IHDR Chunk
            {
                Span<byte> buf = stackalloc byte[13];

                BinaryPrimitives.WriteInt32BigEndian(buf.Slice(0), width);
                BinaryPrimitives.WriteInt32BigEndian(buf.Slice(4), height);
                buf[08] = 8; // depth
                buf[09] = 6; // color (truecolor-alpha)
                buf[10] = 0; // compression
                buf[11] = 0; // filter
                buf[12] = 0; // interlace

                Chunk(writer, "IHDR", buf);
            }

            // IDAT Chunk(s)
            {
                using MemoryStream zlibMemory = new MemoryStream();

                // zlib Header
                zlibMemory.WriteByte(0x78);
                zlibMemory.WriteByte(0x9C);

                uint adler = 1U;

                // filter & deflate data line by line
                using (DeflateStream deflate = new DeflateStream(zlibMemory, CompressionLevel.Fastest, true))
                {
                    fixed (Color* ptr = pixels)
                    {
                        Span<byte> filter = stackalloc byte[1] { 0 };
                        byte* pixelBuffer = (byte*)ptr;

                        for (int y = 0; y < height; y++)
                        {
                            // deflate filter
                            deflate.Write(filter);

                            // update adler checksum
                            adler = Calc.Adler32(adler, filter);

                            // append the row of pixels (in steps, potentially)
                            const int MaxHorizontalStep = 1024;
                            for (int x = 0; x < width; x += MaxHorizontalStep)
                            {
                                var segment = new Span<byte>(pixelBuffer + x * 4, Math.Min(width - x, MaxHorizontalStep) * 4);

                                // delfate the segment of the row
                                deflate.Write(segment);

                                // update adler checksum
                                adler = Calc.Adler32(adler, segment);

                                // write out chunks if we've hit out max IDAT chunk length
                                if (zlibMemory.Position >= MaxIDATChunkLength)
                                    WriteIDAT(writer, zlibMemory, false);
                            }

                            pixelBuffer += width * 4;
                        }
                    }
                }

                // zlib adler32 trailer
                using (BinaryWriter bytes = new BinaryWriter(zlibMemory, Encoding.UTF8, true))
                    bytes.Write(SwapEndian((int)adler));

                // write out remaining chunks
                WriteIDAT(writer, zlibMemory, true);
            }

            // IEND Chunk
            Chunk(writer, "IEND", new Span<byte>());
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte PaethPredictor(byte a, byte b, byte c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);

            if (pa <= pb && pa <= pc)
                return a;
            else if (pb <= pc)
                return b;

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Check(string name, Span<byte> buffer)
        {
            if (buffer.Length < name.Length)
                return false;

            for (int i = 0; i < name.Length; i++)
            {
                if ((char)buffer[i] != name[i])
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SwapEndian(int input)
        {
            if (BitConverter.IsLittleEndian)
            {
                uint value = (uint)input;
                return (int)((value & 0x000000FF) << 24 | (value & 0x0000FF00) << 8 | (value & 0x00FF0000) >> 8 | (value & 0xFF000000) >> 24);
            }

            return input;
        }
    }
}
