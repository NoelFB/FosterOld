using System.Collections.Generic;

namespace Foster.Framework
{
    public class Atlas
    {
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

        public Atlas(Packer packer)
        {
            var output = packer.Pack();
            if (output != null)
            {
                foreach (var page in output.Pages)
                    Pages.Add(Texture.Create(page));

                foreach (var entry in output.Entries.Values)
                {
                    var texture = Pages[entry.Page];
                    var subtexture = new Subtexture(texture, entry.Source, entry.Frame);

                    Subtextures.Add(entry.Name, subtexture);
                }
            }
        }

        /// <summary>
        /// Gets or Sets a Subtexture by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Subtexture this[string name]
        {
            get => Subtextures[name];
            set => Subtextures[name] = value;
        }

    }
}
