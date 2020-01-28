using Foster.Framework.Json;
using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    /// <summary>
    /// A Texture Atlas
    /// </summary>
    public class Atlas : IAsset
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// List of all the Texture Pages of the Atlas
        /// Generally speaking it's ideal to have a single Page per atlas, but that's not always possible.
        /// </summary>
        public readonly List<Texture> Pages = new List<Texture>();

        /// <summary>
        /// A Dictionary of all the Subtextures in this Atlas.
        /// </summary>
        public readonly Dictionary<string, Subtexture> Subtextures = new Dictionary<string, Subtexture>();

        public Atlas() { }

        public Atlas(Packer packer, bool premultiply = false)
        {
            var output = packer.Pack();
            if (output != null)
            {
                foreach (var page in output.Pages)
                {
                    if (premultiply)
                        page.Premultiply();

                    Pages.Add(new Texture(page));
                }

                foreach (var entry in output.Entries.Values)
                {
                    var texture = Pages[entry.Page];
                    var subtexture = new Subtexture(texture, entry.Source, entry.Frame);

                    Subtextures.Add(entry.Name, subtexture);
                }
            }
        }

        public Atlas(JsonReader reader, List<Texture> pages)
        {
            Pages.AddRange(pages);

            if (reader.TryReadObject(out var data))
            {
                foreach (var (page, value) in data.Object)
                {
                    var texture = Pages[int.Parse(page)];

                    if (value.Object != null)
                    {
                        foreach (var (name, subtex) in value.Object)
                        {
                            var source = new Rect(
                                subtex["source"]["x"].Float,
                                subtex["source"]["y"].Float,
                                subtex["source"]["w"].Float,
                                subtex["source"]["h"].Float);

                            var frame = new Rect(
                                subtex["frame"]["x"].Float,
                                subtex["frame"]["y"].Float,
                                subtex["frame"]["w"].Float,
                                subtex["frame"]["h"].Float);

                            Subtextures.Add(name, new Subtexture(texture, source, frame));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets a Subtexture by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Subtexture? this[string name]
        {
            get
            {
                if (Subtextures.TryGetValue(name, out var subtex))
                    return subtex;
                return null;
            }
            set
            {
                if (value != null)
                    Subtextures[name] = value;
            }
        }

    }
}
