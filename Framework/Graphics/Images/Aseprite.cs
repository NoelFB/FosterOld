using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// Parses the Contents of an Aseprite file
    ///
    /// Aseprite File Spec: https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md
    ///
    /// TODO: This is not a true or full implementation, and is missing several features (ex. blendmodes)
    /// 
    /// </summary>
    public class Aseprite
    {
        public enum Modes
        {
            Indexed = 1,
            Grayscale = 2,
            RGBA = 4
        }

        private enum Chunks
        {
            OldPaletteA = 0x0004,
            OldPaletteB = 0x0011,
            Layer = 0x2004,
            Cel = 0x2005,
            CelExtra = 0x2006,
            Mask = 0x2016,
            Path = 0x2017,
            FrameTags = 0x2018,
            Palette = 0x2019,
            UserData = 0x2020,
            Slice = 0x2022
        }

        public Modes Mode { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private int frameCount;

        public readonly List<Layer> Layers = new List<Layer>();
        public readonly List<Frame> Frames = new List<Frame>();
        public readonly List<Tag> Tags = new List<Tag>();
        public readonly List<Slice> Slices = new List<Slice>();

        public Aseprite(string file)
        {
            using var stream = File.Open(file, FileMode.Open);
            Parse(stream);
        }

        public Aseprite(Stream stream)
        {
            Parse(stream);
        }

        #region Data Structures

        public class Frame
        {
            public Aseprite Sprite;
            public int Duration;
            public Bitmap Bitmap;
            public Color[] Pixels => Bitmap.Pixels;
            public List<Cel> Cels;

            public Frame(Aseprite sprite)
            {
                Sprite = sprite;
                Cels = new List<Cel>();
                Bitmap = new Bitmap(sprite.Width, sprite.Height);
            }
        }

        public class Tag
        {
            public enum LoopDirections
            {
                Forward = 0,
                Reverse = 1,
                PingPong = 2
            }

            public string Name = "";
            public LoopDirections LoopDirection;
            public int From;
            public int To;
            public Color Color;
        }

        public interface IUserData
        {
            string UserDataText { get; set; }
            Color UserDataColor { get; set; }
        }

        public class Slice : IUserData
        {
            public int Frame;
            public string Name = "";
            public int OriginX;
            public int OriginY;
            public int Width;
            public int Height;
            public Point2? Pivot;
            public RectInt? NineSlice;
            public string UserDataText { get; set; } = "";
            public Color UserDataColor { get; set; }
        }

        public class Cel : IUserData
        {
            public Layer Layer;
            public Color[] Pixels;
            public Cel? Link;

            public int X;
            public int Y;
            public int Width;
            public int Height;
            public float Alpha;

            public string UserDataText { get; set; } = "";
            public Color UserDataColor { get; set; }

            public Cel(Layer layer, Color[] pixels)
            {
                Layer = layer;
                Pixels = pixels;
            }
        }

        public class Layer : IUserData
        {
            [Flags]
            public enum Flags
            {
                Visible = 1,
                Editable = 2,
                LockMovement = 4,
                Background = 8,
                PreferLinkedCels = 16,
                Collapsed = 32,
                Reference = 64
            }

            public enum Types
            {
                Normal = 0,
                Group = 1
            }

            public Flags Flag;
            public Types Type;
            public string Name = "";
            public int ChildLevel;
            public int BlendMode;
            public float Alpha;
            public bool Visible { get { return Flag.HasFlag(Flags.Visible); } }

            public string UserDataText { get; set; } = "";
            public Color UserDataColor { get; set; }
        }

        #endregion

        #region .ase Parser

        private void Parse(Stream stream)
        {
            var reader = new BinaryReader(stream);

            // wrote these to match the documentation names so it's easier (for me, anyway) to parse
            byte BYTE() => reader.ReadByte();
            ushort WORD() => reader.ReadUInt16();
            short SHORT() => reader.ReadInt16();
            uint DWORD() => reader.ReadUInt32();
            long LONG() => reader.ReadInt32();
            string STRING() => Encoding.UTF8.GetString(BYTES(WORD()));
            byte[] BYTES(int number) => reader.ReadBytes(number);
            void SEEK(int number) => reader.BaseStream.Position += number;

            // Header
            {
                // file size
                DWORD();

                // Magic number (0xA5E0)
                var magic = WORD();
                if (magic != 0xA5E0)
                    throw new Exception("File is not in .ase format");

                // Frames / Width / Height / Color Mode
                frameCount = WORD();
                Width = WORD();
                Height = WORD();
                Mode = (Modes)(WORD() / 8);

                // Other Info, Ignored
                DWORD();       // Flags
                WORD();        // Speed (deprecated)
                DWORD();       // Set be 0
                DWORD();       // Set be 0
                BYTE();        // Palette entry 
                SEEK(3);       // Ignore these bytes
                WORD();        // Number of colors (0 means 256 for old sprites)
                BYTE();        // Pixel width
                BYTE();        // Pixel height
                SEEK(92);      // For Future
            }

            // temporary variables
            var temp = new byte[Width * Height * (int)Mode];
            var palette = new Color[256];
            IUserData? last = null;

            // Frames
            for (int i = 0; i < frameCount; i++)
            {
                var frame = new Frame(this);
                Frames.Add(frame);

                long frameStart, frameEnd;
                int chunkCount;

                // frame header
                {
                    frameStart = reader.BaseStream.Position;
                    frameEnd = frameStart + DWORD();
                    WORD();                  // Magic number (always 0xF1FA)
                    chunkCount = WORD();     // Number of "chunks" in this frame
                    frame.Duration = WORD(); // Frame duration (in milliseconds)
                    SEEK(6);                 // For future (set to zero)
                }

                // chunks
                for (int j = 0; j < chunkCount; j++)
                {
                    long chunkStart, chunkEnd;
                    Chunks chunkType;

                    // chunk header
                    {
                        chunkStart = reader.BaseStream.Position;
                        chunkEnd = chunkStart + DWORD();
                        chunkType = (Chunks)WORD();
                    }

                    // LAYER CHUNK
                    if (chunkType == Chunks.Layer)
                    {
                        // create layer
                        var layer = new Layer();

                        // get layer data
                        layer.Flag = (Layer.Flags)WORD();
                        layer.Type = (Layer.Types)WORD();
                        layer.ChildLevel = WORD();
                        WORD(); // width (unused)
                        WORD(); // height (unused)
                        layer.BlendMode = WORD();
                        layer.Alpha = (BYTE() / 255f);
                        SEEK(3); // for future
                        layer.Name = STRING();

                        last = layer;
                        Layers.Add(layer);
                    }
                    // CEL CHUNK
                    else if (chunkType == Chunks.Cel)
                    {
                        var layer = Layers[WORD()];
                        var x = SHORT();
                        var y = SHORT();
                        var alpha = BYTE() / 255f;
                        var celType = WORD();
                        var width = 0;
                        var height = 0;
                        Color[]? pixels = null;
                        Cel? link = null;

                        SEEK(7);

                        // RAW or DEFLATE
                        if (celType == 0 || celType == 2)
                        {
                            width = WORD();
                            height = WORD();

                            var count = width * height * (int)Mode;
                            if (count > temp.Length)
                                temp = new byte[count];

                            // RAW
                            if (celType == 0)
                            {
                                reader.Read(temp, 0, width * height * (int)Mode);
                            }
                            // DEFLATE
                            else
                            {
                                SEEK(2);

                                using var deflate = new DeflateStream(reader.BaseStream, CompressionMode.Decompress, true);
                                deflate.Read(temp, 0, count);
                            }

                            // get pixel data
                            pixels = new Color[width * height];
                            BytesToPixels(temp, pixels, Mode, palette);

                        }
                        // REFERENCE
                        else if (celType == 1)
                        {
                            var linkFrame = Frames[WORD()];
                            var linkCel = linkFrame.Cels[frame.Cels.Count];

                            width = linkCel.Width;
                            height = linkCel.Height;
                            pixels = linkCel.Pixels;
                            link = linkCel;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        var cel = new Cel(layer, pixels)
                        {
                            X = x,
                            Y = y,
                            Width = width,
                            Height = height,
                            Alpha = alpha,
                            Link = link
                        };

                        // draw to frame if visible
                        if (cel.Layer.Visible)
                            CelToFrame(frame, cel);

                        last = cel;
                        frame.Cels.Add(cel);
                    }
                    // PALETTE CHUNK
                    else if (chunkType == Chunks.Palette)
                    {
                        var size = DWORD();
                        var start = DWORD();
                        var end = DWORD();
                        SEEK(8); // for future

                        for (int p = 0; p < (end - start) + 1; p++)
                        {
                            var hasName = WORD();
                            palette[start + p] = new Color(BYTE(), BYTE(), BYTE(), BYTE()).Premultiply();

                            if (Calc.IsBitSet(hasName, 0))
                                STRING();
                        }
                    }
                    // USERDATA
                    else if (chunkType == Chunks.UserData)
                    {
                        if (last != null)
                        {
                            var flags = (int)DWORD();

                            // has text
                            if (Calc.IsBitSet(flags, 0))
                                last.UserDataText = STRING();

                            // has color
                            if (Calc.IsBitSet(flags, 1))
                                last.UserDataColor = new Color(BYTE(), BYTE(), BYTE(), BYTE()).Premultiply();
                        }
                    }
                    // TAG
                    else if (chunkType == Chunks.FrameTags)
                    {
                        var count = WORD();
                        SEEK(8);

                        for (int t = 0; t < count; t++)
                        {
                            var tag = new Tag();
                            tag.From = WORD();
                            tag.To = WORD();
                            tag.LoopDirection = (Tag.LoopDirections)BYTE();
                            SEEK(8);
                            tag.Color = new Color(BYTE(), BYTE(), BYTE(), (byte)255).Premultiply();
                            SEEK(1);
                            tag.Name = STRING();
                            Tags.Add(tag);
                        }
                    }
                    // SLICE
                    else if (chunkType == Chunks.Slice)
                    {
                        var count = DWORD();
                        var flags = (int)DWORD();
                        DWORD(); // reserved
                        var name = STRING();

                        for (int s = 0; s < count; s++)
                        {
                            var slice = new Slice
                            {
                                Name = name,
                                Frame = (int)DWORD(),
                                OriginX = (int)LONG(),
                                OriginY = (int)LONG(),
                                Width = (int)DWORD(),
                                Height = (int)DWORD()
                            };

                            // 9 slice (ignored atm)
                            if (Calc.IsBitSet(flags, 0))
                            {
                                slice.NineSlice = new RectInt(
                                    (int)LONG(),
                                    (int)LONG(),
                                    (int)DWORD(),
                                    (int)DWORD());
                            }

                            // pivot point
                            if (Calc.IsBitSet(flags, 1))
                                slice.Pivot = new Point2((int)DWORD(), (int)DWORD());
                            
                            last = slice;
                            Slices.Add(slice);
                        }
                    }

                    reader.BaseStream.Position = chunkEnd;
                }

                reader.BaseStream.Position = frameEnd;
            }
        }

        #endregion

        #region Blend Modes

        // More or less copied from Aseprite's source code:
        // https://github.com/aseprite/aseprite/blob/master/src/doc/blend_funcs.cpp

        private delegate void Blend(ref Color dest, Color src, byte opacity);

        private static readonly Blend[] BlendModes = new Blend[]
        {
            // 0 - NORMAL
            (ref Color dest, Color src, byte opacity) =>
            {
                if (src.A != 0)
                {
                    if (dest.A == 0)
                    {
                        dest = src;
                    }
                    else
                    {
                        var sa = MUL_UN8(src.A, opacity);
                        var ra = dest.A + sa - MUL_UN8(dest.A, sa);

                        dest.R = (byte)(dest.R + (src.R - dest.R) * sa / ra);
                        dest.G = (byte)(dest.G + (src.G - dest.G) * sa / ra);
                        dest.B = (byte)(dest.B + (src.B - dest.B) * sa / ra);
                        dest.A = (byte)ra;
                    }

                }
            }
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MUL_UN8(int a, int b)
        {
            var t = (a * b) + 0x80;
            return (((t >> 8) + t) >> 8);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Converts an array of Bytes to an array of Colors, using the specific Aseprite Mode & Palette
        /// </summary>
        private void BytesToPixels(byte[] bytes, Color[] pixels, Modes mode, Color[] palette)
        {
            int len = pixels.Length;
            if (mode == Modes.RGBA)
            {
                for (int p = 0, b = 0; p < len; p++, b += 4)
                {
                    pixels[p].R = (byte)(bytes[b + 0] * bytes[b + 3] / 255);
                    pixels[p].G = (byte)(bytes[b + 1] * bytes[b + 3] / 255);
                    pixels[p].B = (byte)(bytes[b + 2] * bytes[b + 3] / 255);
                    pixels[p].A = bytes[b + 3];
                }
            }
            else if (mode == Modes.Grayscale)
            {
                for (int p = 0, b = 0; p < len; p++, b += 2)
                {
                    pixels[p].R = pixels[p].G = pixels[p].B = (byte)(bytes[b + 0] * bytes[b + 1] / 255);
                    pixels[p].A = bytes[b + 1];
                }
            }
            else if (mode == Modes.Indexed)
            {
                for (int p = 0;  p < len; p++)
                    pixels[p] = palette[bytes[p]];
            }
        }

        /// <summary>
        /// Applies a Cel's pixels to the Frame, using its Layer's BlendMode & Alpha
        /// </summary>
        private void CelToFrame(Frame frame, Cel cel)
        {
            var opacity = (byte)((cel.Alpha * cel.Layer.Alpha) * 255);
            var pxLen = frame.Bitmap.Pixels.Length;

            var blend = BlendModes[0];
            if (cel.Layer.BlendMode < BlendModes.Length)
                blend = BlendModes[cel.Layer.BlendMode];

            for (int sx = Math.Max(0, -cel.X), right = Math.Min(cel.Width, frame.Sprite.Width - cel.X); sx < right; sx++)
            {
                int dx = cel.X + sx;
                int dy = cel.Y * frame.Sprite.Width;

                for (int sy = Math.Max(0, -cel.Y), bottom = Math.Min(cel.Height, frame.Sprite.Height - cel.Y); sy < bottom; sy++, dy += frame.Sprite.Width)
                {
                    if (dx + dy >= 0 && dx + dy < pxLen)
                        blend(ref frame.Bitmap.Pixels[dx + dy], cel.Pixels[sx + sy * cel.Width], opacity);
                }
            }
        }

        /// <summary>
        /// Adds all Aseprite Frames to the Atlas, using the naming format (ex. "mySprite/{0}" where {0} becomes the frame index)
        /// </summary>
        public void Pack(string namingFormat, Packer packer)
        {
            if (!namingFormat.Contains("{0}"))
                throw new Exception("naming format must contain {0} for frame index");

            int frameIndex = 0;
            foreach (var frame in Frames)
            {
                var name = string.Format(namingFormat, frameIndex);
                packer.AddPixels(name, Width, Height, frame.Bitmap.Pixels);

                frameIndex++;
            }

        }

        #endregion
    }
}
