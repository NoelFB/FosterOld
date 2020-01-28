using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class AssetBank
    {

        public delegate IAsset? LoadAsset(Entry entry);

        public class Entry
        {
            public readonly AssetBank Assets;
            public readonly Guid Guid;
            public readonly string Name;
            public readonly Type Type;
            public object? UserData;

            private WeakReference<IAsset>? instance;

            public Entry(AssetBank assets, Guid guid, string name, Type type)
            {
                Assets = assets;
                Guid = guid;
                Name = name;
                Type = type;
            }

            public IAsset? Asset
            {
                get
                {
                    if (instance == null || !instance.TryGetTarget(out var target))
                    {
                        target = Assets.load(this);
                        if (target != null)
                            instance = new WeakReference<IAsset>(target);
                    }

                    if (target != null)
                        target.Guid = Guid;

                    return target;
                }
            }

            public void Unload()
            {
                instance = null;
            }
        }

        private readonly Dictionary<Guid, Entry> entries = new Dictionary<Guid, Entry>();
        private readonly Dictionary<Type, Dictionary<string, Entry>> entriesByName = new Dictionary<Type, Dictionary<string, Entry>>();
        private readonly LoadAsset load;

        public AssetBank(LoadAsset loader)
        {
            load = loader;
        }

        /// <summary>
        /// Adds a new Asset Entry that can be loaded with the given function
        /// </summary>
        public Entry Add<T>(string name) where T : class, IAsset
        {
            return Add(typeof(T), Guid.NewGuid(), name);
        }

        /// <summary>
        /// Adds a new Asset Entry that can be loaded with the given function
        /// </summary>
        public Entry Add(Type type, string name)
        {
            return Add(type, Guid.NewGuid(), name);
        }

        /// <summary>
        /// Adds a new Asset Entry that can be loaded with the given function
        /// </summary>
        public Entry Add<T>(Guid guid, string name) where T : class, IAsset
        {
            return Add(typeof(T), guid, name);
        }

        /// <summary>
        /// Adds a new Asset Entry that can be loaded with the given function
        /// </summary>
        public Entry Add(Type type, Guid guid, string name)
        {
            if (!typeof(IAsset).IsAssignableFrom(type))
                throw new Exception("Type must inherit from IAsset");

            var entry = new Entry(this, guid, name, type);

            if (!entriesByName.TryGetValue(type, out var byName))
                entriesByName[type] = byName = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);

            entries[guid] = entry;
            byName[name] = entry;

            return entry;
        }

        /// <summary>
        /// Removes an Asset Entry
        /// </summary>
        public void Remove(Guid guid)
        {
            if (entries.TryGetValue(guid, out var entry))
            {
                entries.Remove(guid);

                if (entriesByName.TryGetValue(entry.Type, out var byName))
                    byName.Remove(entry.Name);
            }
        }
        
        /// <summary>
        /// Removes an Asset Entry
        /// </summary>
        public void Remove(Entry entry)
        {
            if (entries.ContainsKey(entry.Guid))
            {
                entries.Remove(entry.Guid);

                if (entriesByName.TryGetValue(entry.Type, out var byName))
                    byName.Remove(entry.Name);
            }
        }

        /// <summary>
        /// Removes All Asset Entries
        /// </summary>
        public void Clear()
        {
            entries.Clear();
            entriesByName.Clear();
        }

        /// <summary>
        /// Gets an Asset with the given Guid
        /// </summary>
        public T? Get<T>(Guid guid) where T : class, IAsset
        {
            return GetEntry(guid)?.Asset as T;
        }

        /// <summary>
        /// Gets an Asset with the given Guid
        /// </summary>
        public IAsset? Get(Guid guid)
        {
            return GetEntry(guid)?.Asset;
        }

        /// <summary>
        /// Gets an Asset with the given Name
        /// </summary>
        public T? Get<T>(string? name) where T : class, IAsset
        {
            return GetEntry(typeof(T), name)?.Asset as T;
        }

        /// <summary>
        /// Gets an Asset with the given Name of the given Type
        /// </summary>
        public IAsset? Get(Type type, string? name)
        {
            return GetEntry(type, name)?.Asset;
        }

        /// <summary>
        /// Gets an Asset with the given Guid
        /// </summary>
        public bool Has<T>(Guid guid) where T : class, IAsset
        {
            return GetEntry(guid) != null;
        }

        /// <summary>
        /// Gets an Asset with the given Guid
        /// </summary>
        public bool Has(Guid guid)
        {
            return GetEntry(guid) != null;
        }

        /// <summary>
        /// Gets an Asset with the given Name
        /// </summary>
        public bool Has<T>(string? name) where T : class, IAsset
        {
            return !string.IsNullOrEmpty(name) && GetEntry(typeof(T), name) != null;
        }

        /// <summary>
        /// Gets an Asset with the given Name of the given Type
        /// </summary>
        public bool Has(Type type, string? name)
        {
            return !string.IsNullOrEmpty(name) && GetEntry(type, name) != null;
        }

        /// <summary>
        /// Finds each Asset with a name that starts with the given prefix
        /// </summary>
        public IEnumerable<T> Each<T>(string? prefix = null) where T : class, IAsset
        {
            if (entriesByName.TryGetValue(typeof(T), out var byName))
            {
                foreach (var entry in byName.Values)
                    if (string.IsNullOrEmpty(prefix) || entry.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        if (entry.Asset is T asset)
                            yield return asset;
                    }
            }
        }

        /// <summary>
        /// Finds each Asset with a name that starts with the given prefix
        /// </summary>
        public IEnumerable<IAsset> Each(Type type, string? prefix = null)
        {
            if (entriesByName.TryGetValue(type, out var byName))
            {
                foreach (var entry in byName.Values)
                    if (string.IsNullOrEmpty(prefix) || entry.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var asset = entry.Asset;
                        if (asset != null)
                            yield return asset;
                    }
            }
        }

        /// <summary>
        /// Gets an Asset Entry with the given Guid
        /// </summary>
        public Entry? GetEntry(Guid guid)
        {
            if (entries.TryGetValue(guid, out var entry))
                return entry;

            return null;
        }

        /// <summary>
        /// Gets an Asset Entry with the given Name
        /// </summary>
        public Entry? GetEntry<T>(string? name)
        {
            if (!string.IsNullOrEmpty(name) && entriesByName.TryGetValue(typeof(T), out var byName) && byName.TryGetValue(name, out var entry))
                return entry;

            return null;
        }

        /// <summary>
        /// Gets an Asset Entry with the given Name
        /// </summary>
        public Entry? GetEntry(Type type, string? name)
        {
            if (!string.IsNullOrEmpty(name) && entriesByName.TryGetValue(type, out var byName) && byName.TryGetValue(name, out var entry))
                return entry;

            return null;
        }

    }
}
