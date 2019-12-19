using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{

    public struct AssetHandle
    {

        public AssetBank Bank;
        public Guid Guid;

        public AssetHandle(AssetBank bank, Guid guid)
        {
            Bank = bank;
            Guid = guid;
        }

        public AssetHandle(AssetBank bank, Type type, string name)
        {
            Bank = bank;
            Guid = bank.GetEntry(type, name)?.Guid ?? new Guid();
        }

        public object? Instance => Bank?.Get(Guid);
        public string? Name => Bank?.GetEntry(Guid)?.Name;
        public Type? Type => Bank?.GetEntry(Guid)?.Type;

    }

    public struct AssetHandle<T> where T : class, IAsset
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
            Guid = bank.GetEntry(typeof(T), name)?.Guid ?? new Guid();
        }

        public T? Instance => Bank?.Get<T>(Guid);
        public string? Name => Bank?.GetEntry(Guid)?.Name;

        public static implicit operator T?(AssetHandle<T> aref) => aref.Instance;
        public static implicit operator AssetHandle(AssetHandle<T> aref) => new AssetHandle(aref.Bank, aref.Guid);

    }
}
