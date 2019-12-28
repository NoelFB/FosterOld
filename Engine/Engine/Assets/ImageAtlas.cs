using System;
using System.Collections.Generic;
using Foster.Framework;

namespace Foster.Engine
{
    public class ImageAtlas : IAsset
    {

        public Guid Guid { get; set; }

        public readonly List<Texture> Textures = new List<Texture>();

    }
}
