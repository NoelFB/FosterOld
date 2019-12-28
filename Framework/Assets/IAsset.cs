using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Foster.Framework.Json;

namespace Foster.Framework
{
    public interface IAsset
    {

        /// <summary>
        /// The Guid to this Asset
        /// It is not recommended you modify this value if this Asset belongs to an AssetBank
        /// It can break references between Assets (not good!)
        /// </summary>
        Guid Guid { get; set; }
    }
}
