using Foster.Framework;
using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Foster.Editor
{
    /// <summary>
    /// An Asset Bank that works from the OS File System
    /// </summary>
    public class FileAssetBank : AssetBank
    {

        private struct AssetInfo
        {
            public string Path;
            public DateTime Timestamp;
        }

        private readonly Dictionary<Guid, AssetInfo> info;
        private HashSet<string> existing;

        public readonly string RootPath;

        public FileAssetBank(string rootPath = "")
        {
            info = new Dictionary<Guid, AssetInfo>();
            existing = new HashSet<string>();
            RootPath = rootPath;
        }

        public void Refresh()
        {
            var path = Path.Combine(App.System.Directory, RootPath);
            var removing = new List<Guid>();

            foreach (var pair in info)
            {
                if (!File.Exists(pair.Value.Path))
                {
                    removing.Add(pair.Key);
                    existing.Remove(pair.Value.Path);
                }
                else if (File.GetLastWriteTime(pair.Value.Path) > pair.Value.Timestamp)
                {
                    removing.Add(pair.Key);
                    existing.Remove(pair.Value.Path);
                }
            }

            foreach (var entry in removing)
                Remove(entry);

            AddDirectory(path, path);
        }

        private void AddDirectory(string relative, string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (existing.Contains(file))
                    continue;

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
            var metaPath = filepath + ".meta";
            var hasMeta = false;
            var guid = new Guid();

            // check for meta file
            if (File.Exists(metaPath))
            {
                using var reader = new JsonReader(File.OpenRead(metaPath));
                if (reader.TryReadObject(out var obj) && obj.TryGetValue("guid", out var value) && value.IsString)
                {
                    guid = new Guid(value.String);
                    hasMeta = true;
                }
            }

            // create a default meta file if none exists
            if (!hasMeta)
            {
                guid = Guid.NewGuid();
                using var writer = new JsonWriter(File.OpenWrite(metaPath), false);
                writer.JsonValue(new JsonObject { ["guid"] = guid.ToString() });
            }

            Add(type, guid, name);

            info[guid] = new AssetInfo { Path = filepath, Timestamp = File.GetLastWriteTime(filepath) };
            existing.Add(filepath);
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
