using Foster.Framework;
using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Engine
{
    public class PackedAssetBank : AssetBank
    {

        public void AddPack(string file)
        {

        }

        protected override IAsset? LoadAsset(Guid guid, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
