using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace Foster.Engine
{

    public abstract class AssetBank
    {

        public class Entry
        {
            public readonly AssetBank Bank;
            public readonly Type Type;
            public readonly Guid Guid;
            public readonly string Name;

            private WeakReference<IAsset>? asset;

            public Entry(AssetBank bank, Type assetType, Guid guid, string name)
            {
                Bank = bank;
                Type = assetType;
                Guid = guid;
                Name = name;
            }

            public IAsset? Get()
            {
                if (asset == null || !asset.TryGetTarget(out var target))
                    return Reload();

                return target;
            }

            public IAsset? Reload()
            {
                IAsset? target = Bank.LoadAsset(Guid, Type);

                if (target != null)
                {
                    target.Guid = Guid;

                    if (asset == null)
                        asset = new WeakReference<IAsset>(target);
                    else
                        asset.SetTarget(target);
                }

                return target;
            }

            public void Unload()
            {
                if (asset != null && asset.TryGetTarget(out var target))
                    asset = null;
            }
        }

        private readonly Dictionary<Type, Dictionary<string, Entry>> byName;
        private readonly Dictionary<Guid, Entry> byGuid;

        protected AssetBank()
        {
            byName = new Dictionary<Type, Dictionary<string, Entry>>();
            byGuid = new Dictionary<Guid, Entry>();
        }

        protected abstract IAsset? LoadAsset(Guid guid, Type type);

        protected void Add<T>(Guid guid, string name)
        {
            Add(typeof(T), guid, name);
        }

        protected void Add(Type type, Guid guid, string name)
        {
            var entry = new Entry(this, type, guid, name);
            if (!byName.TryGetValue(type, out var nameDictionary))
                byName[type] = nameDictionary = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);

            nameDictionary[name] = entry;
            byGuid[guid] = entry;
        }

        protected void Remove(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var entry))
            {
                byGuid.Remove(guid);
                byName[entry.Type].Remove(entry.Name);
            }
        }

        public T? Get<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var entry))
                return entry.Get() as T;
            return null;
        }

        public IAsset? Get(Type type, string name)
        {
            if (byName.TryGetValue(type, out var dictionary) && dictionary.TryGetValue(name, out var entry))
                return entry.Get();
            return null;
        }

        public T? Get<T>(Guid guid) where T : class, IAsset
        {
            if (byGuid.TryGetValue(guid, out var entry))
                return entry.Get() as T;
            return null;
        }

        public IAsset? Get(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var entry))
                return entry.Get();
            return null;
        }

        public Entry? GetEntry(Type type, string name)
        {
            if (byName.TryGetValue(type, out var dictionary) && dictionary.TryGetValue(name, out var entry))
                return entry;
            return null;
        }

        public Entry? GetEntry(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var entry))
                return entry;
            return null;
        }

        public IEnumerable<T> Each<T>(string? prefix = null) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary))
            {
                foreach (var entry in dictionary.Values)
                    if (string.IsNullOrWhiteSpace(prefix) || entry.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var asset = entry.Get() as T;
                        if (asset != null)
                            yield return asset;
                    }
            }
        }

        public void Unload<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var entry))
                entry.Unload();
        }

        public void Unload(Type type, string name)
        {
            if (byName.TryGetValue(type, out var dictionary) && dictionary.TryGetValue(name, out var entry))
                entry.Unload();
        }

        public void Unload(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var entry))
                entry.Unload();
        }

        public void UnloadAll<T>()
        {
            UnloadAll(typeof(T));
        }

        public void UnloadAll(Type type)
        {
            if (byName.TryGetValue(type, out var dictionary))
            {
                foreach (var entry in dictionary.Values)
                    entry.Unload();
            }
        }

        public void UnloadAll()
        {
            foreach (var entry in byGuid.Values)
                entry.Unload();
        }

    }
}
