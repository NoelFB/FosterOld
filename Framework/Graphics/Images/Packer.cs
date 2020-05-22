using System;
using System.Collections.Generic;
using System.IO;

namespace Foster.Framework
{
    /// <summary>
    /// The Packer takes source image data and packs them into large texture pages that can then be used for Atlases
    /// This is useful for sprite fonts, sprite sheets, etc.
    /// </summary>
    public class Packer
    {

        /// <summary>
        /// A single packed Entry
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// The Name of the Entry
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// The corresponding image page of the Entry
            /// </summary>
            public readonly int Page;

            /// <summary>
            /// The Source Rectangle
            /// </summary>
            public readonly RectInt Source;

            /// <summary>
            /// The Frame Rectangle. This is the size of the image before it was packed
            /// </summary>
            public readonly RectInt Frame;

            public Entry(string name, int page, RectInt source, RectInt frame)
            {
                Name = name;
                Page = page;
                Source = source;
                Frame = frame;
            }
        }

        /// <summary>
        /// Stores the Packed result of the Packer
        /// </summary>
        public class Output
        {
            public readonly List<Bitmap> Pages = new List<Bitmap>();
            public readonly Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();
        }

        /// <summary>
        /// The Packed Output
        /// This is null if the Packer has not yet been packed
        /// </summary>
        public Output Packed { get; private set; } = new Output();

        /// <summary>
        /// Whether the Packer has unpacked source data
        /// </summary>
        public bool HasUnpackedData { get; private set; }

        /// <summary>
        /// Whether to trim transparency from the source images
        /// </summary>
        public bool Trim = true;

        /// <summary>
        /// Maximum Texture Size. If the packed data is too large it will be split into multiple pages
        /// </summary>
        public int MaxSize = 8192;

        /// <summary>
        /// Image Padding
        /// </summary>
        public int Padding = 1;

        /// <summary>
        /// Power of Two
        /// </summary>
        public bool PowerOfTwo = false;

        /// <summary>
        /// This will check each image to see if it's a duplicate of an already packed image. 
        /// It will still add the entry, but not the duplicate image data.
        /// </summary>
        public bool CombineDuplicates = false;

        /// <summary>
        /// The total number of source images
        /// </summary>
        public int SourceImageCount => sources.Count;

        private class Source
        {
            public string Name;
            public RectInt Packed;
            public RectInt Frame;
            public Color[]? Buffer;
            public Source? DuplicateOf;
            public bool Empty => Packed.Width <= 0 || Packed.Height <= 0;

            public Source(string name)
            {
                Name = name;
            }
        }

        private readonly List<Source> sources = new List<Source>();
        private readonly Dictionary<int, Source> duplicateLookup = new Dictionary<int, Source>();

        public void AddBitmap(string name, Bitmap bitmap)
        {
            if (bitmap != null)
                AddPixels(name, bitmap.Width, bitmap.Height, new ReadOnlySpan<Color>(bitmap.Pixels));
        }

        public void AddFile(string name, string path)
        {
            using var stream = File.OpenRead(path);
            AddBitmap(name, new Bitmap(stream));
        }

        public void AddPixels(string name, int width, int height, ReadOnlySpan<Color> pixels)
        {
            HasUnpackedData = true;

            var source = new Source(name);
            int top = 0, left = 0, right = width, bottom = height;

            // trim
            if (Trim)
            {
                // TOP:
                for (int y = 0; y < height; y++)
                    for (int x = 0, s = y * width; x < width; x++, s++)
                        if (pixels[s].A > 0)
                        {
                            top = y;
                            goto LEFT;
                        }
                    LEFT:
                for (int x = 0; x < width; x++)
                    for (int y = top, s = x + y * width; y < height; y++, s += width)
                        if (pixels[s].A > 0)
                        {
                            left = x;
                            goto RIGHT;
                        }
                    RIGHT:
                for (int x = width - 1; x >= left; x--)
                    for (int y = top, s = x + y * width; y < height; y++, s += width)
                        if (pixels[s].A > 0)
                        {
                            right = x + 1;
                            goto BOTTOM;
                        }
                    BOTTOM:
                for (int y = height - 1; y >= top; y--)
                    for (int x = left, s = x + y * width; x < right; x++, s++)
                        if (pixels[s].A > 0)
                        {
                            bottom = y + 1;
                            goto END;
                        }
                    END:;
            }

            // determine sizes
            // there's a chance this image was empty in which case we have no width / height
            if (left <= right && top <= bottom)
            {
                var isDuplicate = false;

                if (CombineDuplicates)
                {
                    var hash = 0;
                    for (int x = left; x < right; x++)
                        for (int y = top; y < bottom; y++)
                            hash = ((hash << 5) + hash) + (int)pixels[x + y * width].ABGR;

                    if (duplicateLookup.TryGetValue(hash, out var duplicate))
                    {
                        source.DuplicateOf = duplicate;
                        isDuplicate = true;
                    }
                    else
                    {
                        duplicateLookup.Add(hash, source);
                    }
                }

                source.Packed = new RectInt(0, 0, right - left, bottom - top);
                source.Frame = new RectInt(-left, -top, width, height);

                if (!isDuplicate)
                {
                    source.Buffer = new Color[source.Packed.Width * source.Packed.Height];

                    // copy our trimmed pixel data to the main buffer
                    for (int i = 0; i < source.Packed.Height; i++)
                    {
                        var run = source.Packed.Width;
                        var from = pixels.Slice(left + (top + i) * width, run);
                        var to = new Span<Color>(source.Buffer, i * run, run);

                        from.CopyTo(to);
                    }
                }
            }
            else
            {
                source.Packed = new RectInt();
                source.Frame = new RectInt(0, 0, width, height);
            }

            sources.Add(source);
        }

        private struct PackingNode
        {
            public bool Used;
            public RectInt Rect;
            public unsafe PackingNode* Right;
            public unsafe PackingNode* Down;
        };

        public unsafe Output Pack()
        {
            // Already been packed
            if (!HasUnpackedData)
                return Packed;

            // Reset
            Packed = new Output();
            HasUnpackedData = false;

            // Nothing to pack
            if (sources.Count <= 0)
                return Packed;

            // sort the sources by size
            sources.Sort((a, b) => b.Packed.Width * b.Packed.Height - a.Packed.Width * a.Packed.Height);

            // make sure the largest isn't too large
            if (sources[0].Packed.Width > MaxSize || sources[0].Packed.Height > MaxSize)
                throw new Exception("Source image is larger than max atlas size");

            // TODO: why do we sometimes need more than source images * 3?
            // for safety I've just made it 4 ... but it should really only be 3?

            int nodeCount = sources.Count * 4;
            Span<PackingNode> buffer = (nodeCount <= 2000 ?
                stackalloc PackingNode[nodeCount] :
                new PackingNode[nodeCount]);

            var padding = Math.Max(0, Padding);

            // using pointer operations here was faster
            fixed (PackingNode* nodes = buffer)
            {
                int packed = 0, page = 0;
                while (packed < sources.Count)
                {
                    if (sources[packed].Empty)
                    {
                        packed++;
                        continue;
                    }

                    var from = packed;
                    var nodePtr = nodes;
                    var rootPtr = ResetNode(nodePtr++, 0, 0, sources[from].Packed.Width + padding, sources[from].Packed.Height + padding);

                    while (packed < sources.Count)
                    {
                        if (sources[packed].Empty || sources[packed].DuplicateOf != null)
                        {
                            packed++;
                            continue;
                        }

                        int w = sources[packed].Packed.Width + padding;
                        int h = sources[packed].Packed.Height + padding;
                        var node = FindNode(rootPtr, w, h);

                        // try to expand
                        if (node == null)
                        {
                            bool canGrowDown = (w <= rootPtr->Rect.Width) && (rootPtr->Rect.Height + h < MaxSize);
                            bool canGrowRight = (h <= rootPtr->Rect.Height) && (rootPtr->Rect.Width + w < MaxSize);
                            bool shouldGrowRight = canGrowRight && (rootPtr->Rect.Height >= (rootPtr->Rect.Width + w));
                            bool shouldGrowDown = canGrowDown && (rootPtr->Rect.Width >= (rootPtr->Rect.Height + h));

                            if (canGrowDown || canGrowRight)
                            {
                                // grow right
                                if (shouldGrowRight || (!shouldGrowDown && canGrowRight))
                                {
                                    var next = ResetNode(nodePtr++, 0, 0, rootPtr->Rect.Width + w, rootPtr->Rect.Height);
                                    next->Used = true;
                                    next->Down = rootPtr;
                                    next->Right = node = ResetNode(nodePtr++, rootPtr->Rect.Width, 0, w, rootPtr->Rect.Height);
                                    rootPtr = next;
                                }
                                // grow down
                                else
                                {
                                    var next = ResetNode(nodePtr++, 0, 0, rootPtr->Rect.Width, rootPtr->Rect.Height + h);
                                    next->Used = true;
                                    next->Down = node = ResetNode(nodePtr++, 0, rootPtr->Rect.Height, rootPtr->Rect.Width, h);
                                    next->Right = rootPtr;
                                    rootPtr = next;
                                }
                            }
                        }

                        // doesn't fit in this page
                        if (node == null)
                            break;

                        // add
                        node->Used = true;
                        node->Down = ResetNode(nodePtr++, node->Rect.X, node->Rect.Y + h, node->Rect.Width, node->Rect.Height - h);
                        node->Right = ResetNode(nodePtr++, node->Rect.X + w, node->Rect.Y, node->Rect.Width - w, h);

                        sources[packed].Packed.X = node->Rect.X;
                        sources[packed].Packed.Y = node->Rect.Y;

                        packed++;
                    }

                    // get page size
                    int pageWidth, pageHeight;
                    if (PowerOfTwo)
                    {
                        pageWidth = 2;
                        pageHeight = 2;
                        while (pageWidth < rootPtr->Rect.Width)
                            pageWidth *= 2;
                        while (pageHeight < rootPtr->Rect.Height)
                            pageHeight *= 2;
                    }
                    else
                    {
                        pageWidth = rootPtr->Rect.Width;
                        pageHeight = rootPtr->Rect.Height;
                    }

                    // create each page
                    {
                        var bmp = new Bitmap(pageWidth, pageHeight);
                        Packed.Pages.Add(bmp);

                        // create each entry for this page and copy its image data
                        for (int i = from; i < packed; i++)
                        {
                            var source = sources[i];

                            // do not pack duplicate entries yet
                            if (source.DuplicateOf == null)
                            {
                                Packed.Entries[source.Name] = new Entry(source.Name, page, source.Packed, source.Frame);

                                if (!source.Empty)
                                    bmp.SetPixels(source.Packed, source.Buffer);
                            }
                        }
                    }

                    page++;
                }

            }

            // make sure duplicates have entries
            if (CombineDuplicates)
            {
                foreach (var source in sources)
                {
                    if (source.DuplicateOf != null)
                    {
                        var entry = Packed.Entries[source.DuplicateOf.Name];
                        Packed.Entries[source.Name] = new Entry(source.Name, entry.Page, entry.Source, entry.Frame);
                    }
                }
            }

            return Packed;

            static unsafe PackingNode* FindNode(PackingNode* root, int w, int h)
            {
                if (root->Used)
                {
                    var r = FindNode(root->Right, w, h);
                    return (r != null ? r : FindNode(root->Down, w, h));
                }
                else if (w <= root->Rect.Width && h <= root->Rect.Height)
                {
                    return root;
                }

                return null;
            }

            static unsafe PackingNode* ResetNode(PackingNode* node, int x, int y, int w, int h)
            {
                node->Used = false;
                node->Rect = new RectInt(x, y, w, h);
                node->Right = null;
                node->Down = null;
                return node;
            }
        }

        /// <summary>
        /// Removes all source data and removes the Packed Output
        /// </summary>
        public void Clear()
        {
            sources.Clear();
            duplicateLookup.Clear();
            Packed = new Output();
            HasUnpackedData = false;
        }

    }
}
