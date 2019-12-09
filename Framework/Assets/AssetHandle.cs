using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public struct AssetHandle<T> where T : Asset
    {

        public AssetBank Bank;
        public Guid Guid;

        public AssetHandle(AssetBank bank, Guid guid)
        {
            Bank = bank;
            Guid = guid;
        }

        public AssetHandle(AssetBank bank, string name)
        {
            Bank = bank;
            Guid = bank.GuidOf<T>(name) ?? new Guid();
        }

        public T? Instance => Bank.Get<T>(Guid);

        public static implicit operator T?(AssetHandle<T> aref) => aref.Instance;

    }
}
