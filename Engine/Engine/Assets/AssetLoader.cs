using System;
using System.IO;
using Foster.Framework;
using Foster.Framework.Json;

namespace Foster.Engine
{
    public abstract class AssetLoader
    {
        public readonly string[] FileExtensions;
        public readonly Type AssetType;

        protected AssetLoader(Type assetType, params string[] fileExtensions)
        {
            AssetType = assetType;
            FileExtensions = fileExtensions;
        }

        public abstract IAsset Load(AssetBank bank, Stream stream, JsonObject? metadata);
    }

    public abstract class AssetLoader<T> : AssetLoader where T : IAsset
    {
        protected AssetLoader(params string[] fileExtensions) : base(typeof(T), fileExtensions)
        {

        }
    }
}