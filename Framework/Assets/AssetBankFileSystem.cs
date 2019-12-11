using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// An Asset Bank that syncs with a File System
    /// </summary>
    public class AssetBankFileSystem : AssetBank
    {

        private struct AssetInfo
        {
            public string Path;
            public DateTime Timestamp;
        }

        private readonly Dictionary<Guid, AssetInfo> info;

        public readonly string RootPath;

        public AssetBankFileSystem(string rootPath = "")
        {
            info = new Dictionary<Guid, AssetInfo>();
            RootPath = rootPath;

            var path = Path.Combine(App.System.Directory, RootPath);
            AddDirectory(path, path);
        }

        public void Sync()
        {
            // removing missing assets
            // ...

            // add newly created assets
            // ...

            // reload assets that changed
            // ...
        }

        private void AddDirectory(string root, string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var ext = ((ReadOnlySpan<char>)Path.GetExtension(file));
                if (ext.Length > 0 && ext[0] == '.')
                    ext = ext.Slice(1);

                var added = false;
                foreach (var loader in AssetLoaders.Loaders)
                {
                    foreach (var extension in loader.Extensions)
                        if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            var name = GetName(root, file);
                            AddEntry(loader.Type, name, file);
                            added = true;
                            break;
                        }

                    if (added)
                        break;
                }
            }

            foreach (var dir in Directory.EnumerateDirectories(path))
                AddDirectory(root, dir);
        }

        private void AddEntry(Type type, string name, string filepath)
        {
            var meta = filepath + ".meta";
            var guid = Guid.NewGuid();

            if (File.Exists(meta))
            {
                guid = new Guid(File.ReadAllText(meta));
            }

            Add(type, guid, name);

            Console.WriteLine(type.Name + " : " + name + " : " + guid.ToString());

            info[guid] = new AssetInfo { Path = filepath, Timestamp = File.GetLastWriteTime(filepath) };
        }

        private unsafe string GetName(ReadOnlySpan<char> root, ReadOnlySpan<char> path)
        {
            Span<char> r = stackalloc char[root.Length];
            root.CopyTo(r);

            Span<char> p = stackalloc char[path.Length];
            path.CopyTo(p);

            // normalize the paths
            r = Calc.NormalizePath(r);
            p = Calc.NormalizePath(p);

            // find the relative path
            p = Calc.RelativePath(r, p);

            // remove file extension
            var ext = p.LastIndexOf('.');
            if (ext >= 0)
                p = p.Slice(0, ext);

            return p.ToString();
        }

        protected override bool GetAssetStream(Guid guid, out Stream? stream, out JsonObject? metadata)
        {
            stream = null;
            metadata = null;

            var path = info[guid].Path;
            if (File.Exists(path))
            {
                stream = File.OpenRead(path);
                return true;
            }

            return false;
        }
    }
}
