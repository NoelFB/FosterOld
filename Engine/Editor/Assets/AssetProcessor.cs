using System;
using Foster.Engine;

namespace Foster.Editor
{

    public abstract class AssetProcessor
    {
        public abstract void Changed(AssetBank bank, Guid guid, string name, string path);
    }

    public abstract class AssetProcessor<T> : AssetProcessor where T : IAsset
    {

    }
}
