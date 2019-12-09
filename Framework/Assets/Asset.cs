using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    public abstract class Asset : IDisposable
    {

        /// <summary>
        /// The Guid to this Asset
        /// It is not recommended you modify this value if this Asset belongs to an AssetBank
        /// It can break references between Assets (not good!)
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Disposes the contents of the Asset
        /// </summary>
        public virtual void Dispose() { }
    }
}
