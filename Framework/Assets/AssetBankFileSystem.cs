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

        private void AddDirectory(string relative, string path)
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
                            var name = GetName(relative, file);
                            AddEntry(loader.Type, name, file);
                            added = true;
                            break;
                        }

                    if (added)
                        break;
                }
            }

            foreach (var dir in Directory.EnumerateDirectories(path))
                AddDirectory(relative, dir);
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

        private unsafe string GetName(ReadOnlySpan<char> relative, ReadOnlySpan<char> path)
        {
            Span<char> r = stackalloc char[relative.Length];
            Span<char> p = stackalloc char[path.Length];
            
            relative.CopyTo(r);
            path.CopyTo(p);

            // normalize
            r = Normalize(r);
            p = Normalize(p);

            // get relative
            var start = 0;
            while (start < p.Length && start < r.Length && char.ToLower(p[start]) == char.ToLower(r[start]))
                start++;
            p = p.Slice(start);

            // trim any slashes
            p = p.Trim('/');

            // remove file extension
            var ext = p.LastIndexOf('.');
            if (ext >= 0)
                p = p.Slice(0, ext);

            return p.ToString();

            static Span<char> Normalize(Span<char> ptr)
            {
                for (int i = 0; i < ptr.Length; i++)
                    if (ptr[i] == '\\') ptr[i] = '/';

                int length = ptr.Length;
                for (int i = 1, t = 1, l = length; t < l; i++, t++)
                {
                    if (ptr[t - 1] == '/' && ptr[t] == '/')
                    {
                        i--;
                        length--;
                    }
                    else
                        ptr[i] = ptr[t];
                }

                return ptr.Slice(0, length);
            }
        }

        protected override bool GetAssetStream(Guid guid, out Stream? stream, out AssetMeta? metadata)
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
