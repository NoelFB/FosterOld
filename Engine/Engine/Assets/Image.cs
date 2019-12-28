using System;
using Foster.Framework;
using Foster.Framework.Json;

namespace Foster.Engine
{
    public class Image : Subtexture, IAsset
    {
        public Guid Guid { get; set; }

        public ImageAtlas? Atlas { get; internal set; }
        public int AtlasPage { get; internal set; }

        public Image(ImageAtlas atlas, int page, Rect source, Rect frame) : base(atlas.Textures[page], source, frame)
        {
            Atlas = atlas;
            AtlasPage = page;
        }

        public Image(Texture texture, Rect source, Rect frame)
            : base(texture, source, frame)
        {

        }
    }
}
