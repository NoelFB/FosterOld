using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{

    public static class AssetLoaders
    {

        public delegate object LoadFunc(AssetBank bank, Stream stream, JsonObject? metadata);

        public class Loader
        {
            /// <summary>
            /// The type of Asset this loads
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// The File Extensions associated with this Asset
            /// </summary>
            public readonly string[] Extensions;

            /// <summary>
            /// The Load Function for this Asset
            /// </summary>
            public readonly LoadFunc Load;

            public Loader(Type type, string[] extensions, LoadFunc loader)
            {
                Type = type;
                Extensions = extensions;
                Load = loader;
            }
        }

        private static readonly Dictionary<Type, Loader> loaders = new Dictionary<Type, Loader>();

        /// <summary>
        /// Registers an Asset Loader for the given Asset Type
        /// Note that only one asset loader can exist per type
        /// </summary>
        public static void Register<T>(string[] extensions, LoadFunc loadFunc) where T : IAsset
        {
            loaders[typeof(T)] = new Loader(typeof(T), extensions, loadFunc);
        }

        /// <summary>
        /// Loads an Asset of the given Type
        /// </summary>
        public static T? Load<T>(AssetBank bank, Stream stream, JsonObject? metadata) where T : IAsset
        {
            if (loaders.TryGetValue(typeof(T), out var type) && type != null)
                return (type.Load(bank, stream, metadata) as T);
            return null;
        }

        /// <summary>
        /// Enumerates over all Asset Loaders
        /// </summary>
        public static IEnumerable<Loader> Loaders
        {
            get
            {
                foreach (var type in loaders.Values)
                    yield return type;
            }
        }

        /// <summary>
        /// Registers the Default Asset Loaders that are built into Foster
        /// </summary>
        internal static void RegisterDefaultLoaders()
        {
            // Texture
            Register<Texture>(new [] { "png", "jpg", "jpeg", "bmp" }, (bank, stream, metadata) =>
            {
                var tex =  new Texture(new Bitmap(stream));

                return tex;
            });;
        }
    }
}
