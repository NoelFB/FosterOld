using Foster.Framework;
using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Runtime
{
    public class PackedAssetBank : AssetBank
    {

        public void AddPack(string file)
        {

        }

        protected override bool GetAssetStream(Guid guid, out Stream? stream, out JsonObject? metadata)
        {
            throw new NotImplementedException();
        }
    }
}
