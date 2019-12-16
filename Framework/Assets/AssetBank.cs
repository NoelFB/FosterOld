using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace Foster.Framework
{

    public abstract class AssetBank
    {

        private class Resource
        {
            public AssetBank Bank;
            public Type Type;
            public Guid Guid;
            public string Name;
            public WeakReference<IAsset>? Asset;

            public Resource(AssetBank bank, Type assetType, Guid guid, string name)
            {
                Bank = bank;
                Type = assetType;
                Guid = guid;
                Name = name;
            }

            public T? Get<T>() where T : class, IAsset
            {
                if (Asset == null || !Asset.TryGetTarget(out var target))
                    return Reload<T>();

                return target as T;
            }

            public T? Reload<T>() where T : class, IAsset
            {
                T? target = null;

                if (Bank.GetAssetStream(Guid, out var stream, out var metadata) && stream != null)
                {
                    target = AssetLoaders.Load<T>(Bank, stream, metadata);

                    if (target != null)
                    {
                        target.Guid = Guid;

                        if (Asset == null)
                            Asset = new WeakReference<IAsset>(target);
                        else
                            Asset.SetTarget(target);
                    }
                }

                if (stream != null)
                    stream.Dispose();

                return target;
            }

            public void Dispose()
            {
                if (Asset != null && Asset.TryGetTarget(out var target))
                {
                    target.Dispose();
                    Asset = null;
                }
            }
        }

        private readonly Dictionary<Type, Dictionary<string, Resource>> byName;
        private readonly Dictionary<Guid, Resource> byGuid;

        public AssetBank()
        {
            byName = new Dictionary<Type, Dictionary<string, Resource>>();
            byGuid = new Dictionary<Guid, Resource>();
        }

        /// <summary>
        /// Finds and Opens a Stream for an Asset
        /// </summary>
        protected abstract bool GetAssetStream(Guid guid, out Stream? stream, out JsonObject? metadata);

        /// <summary>
        /// Adds an Asset to the Bank
        /// </summary>
        protected void Add<T>(Guid guid, string name)
        {
            Add(typeof(T), guid, name);
        }

        /// <summary>
        /// Adds an Asset to the Bank
        /// </summary>
        protected void Add(Type type, Guid guid, string name)
        {
            var resource = new Resource(this, type, guid, name);
            if (!byName.TryGetValue(type, out var nameDictionary))
                byName[type] = nameDictionary = new Dictionary<string, Resource>();

            nameDictionary[name] = resource;
            byGuid[guid] = resource;
        }

        /// <summary>
        /// Removes an Asset from the Bank
        /// </summary>
        protected void Remove(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var resource))
            {
                byGuid.Remove(guid);
                byName[resource.Type].Remove(resource.Name);
            }
        }

        /// <summary>
        /// Finds the Name of an Asset
        /// </summary>
        public string? NameOf(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var resource))
                return resource.Name;
            return null;
        }

        /// <summary>
        /// Finds the Type of an Asset
        /// </summary>
        public Type? TypeOf(Guid guid)
        {
            if (byGuid.TryGetValue(guid, out var resource))
                return resource.Type;
            return null;
        }

        /// <summary>
        /// Finds the Guid of the Asset with the given name
        /// </summary>
        public Guid? GuidOf<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var resource))
                return resource.Guid;
            return null;
        }

        /// <summary>
        /// Gets a given Asset
        /// </summary>
        public T? Get<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var resource))
                return resource.Get<T>();
            return null;
        }

        /// <summary>
        /// Gets a given Asset
        /// </summary>
        public T? Get<T>(Guid guid) where T : class, IAsset
        {
            if (byGuid.TryGetValue(guid, out var resource))
                return resource.Get<T>();
            return null;
        }

        /// <summary>
        /// Gets a given Asset and forces a Reload
        /// </summary>
        public T? Reload<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var resource))
                return resource.Reload<T>();
            return null;
        }

        /// <summary>
        /// Gets a given Asset and forces a Reload
        /// </summary>
        public T? Reload<T>(Guid guid) where T : class, IAsset
        {
            if (byGuid.TryGetValue(guid, out var resource))
                return resource.Reload<T>();
            return null;
        }

        /// <summary>
        /// Finds all Assets of the given type, with the given prefix
        /// </summary>
        public IEnumerable<T> Each<T>(string? prefix = null) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary))
            {
                foreach (var resource in dictionary.Values)
                    if (string.IsNullOrWhiteSpace(prefix) || resource.Name.StartsWith(prefix))
                    {
                        var asset = resource.Get<T>();
                        if (asset != null)
                            yield return asset;
                    }
            }
        }

        /// <summary>
        /// Makes an AssetHandle to an Asset
        /// </summary>
        public AssetHandle<T> Handle<T>(string name) where T : class, IAsset
        {
            return new AssetHandle<T>(this, name);
        }

        /// <summary>
        /// Makes an AssetHandle to an Asset
        /// </summary>
        public AssetHandle<T> Handle<T>(Guid guid) where T : class, IAsset
        {
            return new AssetHandle<T>(this, guid);
        }

        /// <summary>
        /// Disposes the Asset, forcing a Reload the next time it's requested
        /// </summary>
        public void Dispose<T>(string name) where T : class, IAsset
        {
            if (byName.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var resource))
                resource.Dispose();
        }

        /// <summary>
        /// Disposes the Asset, forcing a Reload the next time it's requested
        /// </summary>
        public void Dispose<T>(Guid guid) where T : class, IAsset
        {
            if (byGuid.TryGetValue(guid, out var resource))
                resource.Dispose();
        }

        /// <summary>
        /// Disposes all Assets
        /// </summary>
        public void Dispose()
        {
            foreach (var resource in byGuid.Values)
                resource.Dispose();
        }

    }
}
