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
    public class ProjectAssetBank : AssetBank, IDisposable
    {

        public readonly string AssetsPath;

        private readonly Dictionary<string, Guid> pathToGuid = new Dictionary<string, Guid>();
        private readonly Dictionary<Guid, string> guidToPath = new Dictionary<Guid, string>();
        private readonly List<(string, WatcherChangeTypes)> markedFiles = new List<(string, WatcherChangeTypes)>();
        private readonly FileSystemWatcher watcher;

        public bool IsWaitingForSync => markedFiles.Count > 0;

        public ProjectAssetBank(string assetsPath)
        {
            AssetsPath = assetsPath;

            if (!Directory.Exists(AssetsPath))
                Directory.CreateDirectory(AssetsPath);

            watcher = new FileSystemWatcher(AssetsPath, "*.*")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
            };

            watcher.Created += FileChanged;
            watcher.Changed += FileChanged;
            watcher.Deleted += FileChanged;
            watcher.Renamed += FileRenamed;
        }

        public void StartWatching()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            var ext = Path.GetExtension(e.FullPath);

            if (ext != null && !ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                MarkFile(e.FullPath, e.ChangeType);
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var ext = Path.GetExtension(e.FullPath);

            if (ext != null && !ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) && !!ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                // TODO:
                // actually mark the file as renamed, so it can hande this more gracefully?
                // ex. it could automatically hook up the guid again, maybe?

                MarkFile(e.OldFullPath, WatcherChangeTypes.Deleted);
                MarkFile(e.FullPath, WatcherChangeTypes.Created);
            }
        }

        public void MarkFile(string fullPath, WatcherChangeTypes mark)
        {
            if (mark == WatcherChangeTypes.Created || pathToGuid.ContainsKey(GetRelativePath(fullPath)))
                markedFiles.Add((fullPath, mark));
        }

        public void FindAllFiles()
        {
            // we don't care about any marked files because we're doing a full sweep
            markedFiles.Clear();

            foreach (var file in Directory.EnumerateFiles(AssetsPath, "*.*", SearchOption.AllDirectories))
                AddFile(file);
        }
        
        public void SyncFiles()
        {
            int count = markedFiles.Count;
            for (int i = 0; i < count; i ++)
            {
                var path = markedFiles[i].Item1;
                var mark = markedFiles[i].Item2;

                if (mark == WatcherChangeTypes.Created)
                    AddFile(path);
                else if (mark == WatcherChangeTypes.Deleted)
                    RemoveFile(path);
                else if (mark == WatcherChangeTypes.Changed)
                    UpdateFile(path);
            }

            markedFiles.RemoveRange(0, count);
        }

        private bool AddFile(string fullPath)
        {
            var ext = ((ReadOnlySpan<char>)Path.GetExtension(fullPath));
            if (ext.Length > 0 && ext[0] == '.')
                ext = ext.Slice(1);

            // skip meta files
            if (ext.Equals("meta", StringComparison.OrdinalIgnoreCase))
                return false;

            // find the type based on the extension
            Type? type = null;
            foreach (var loader in AssetLoaders.Loaders)
            {
                foreach (var extension in loader.Extensions)
                    if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        type = loader.Type;
                        break;
                    }
                
                if (type != null)
                    break;
            }

            // we can't figure out what type this should be ... so ignore it
            if (type == null)
                return false;

            // get the name, and unload if it already exists
            var name = GetName(fullPath);
            if (Exists(type, name))
            {
                Unload(type, name);
                return false;
            }

            var relative = GetRelativePath(fullPath);
            var metaPath = fullPath + ".meta";
            var guid = new Guid();

            // check for meta file
            var hasMeta = false;
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

            pathToGuid[relative] = guid;
            guidToPath[guid] = relative;
            return true;
        }

        private void UpdateFile(string fullPath)
        {
            var relative = GetRelativePath(fullPath);
            if (pathToGuid.TryGetValue(relative, out var guid))
                Unload(guid);
        }

        private void RemoveFile(string fullPath)
        {
            var relative = GetRelativePath(fullPath);
            if (pathToGuid.TryGetValue(relative, out var guid))
            {
                guidToPath.Remove(guid);
                pathToGuid.Remove(relative);
                Remove(guid);
            }
        }

        protected override bool GetAssetStream(Guid guid, out Stream? stream, out JsonObject? metadata)
        {
            stream = null;
            metadata = null;

            var path = Path.Combine(AssetsPath, guidToPath[guid]);
            if (File.Exists(path))
            {
                stream = File.OpenRead(path);
                return true;
            }

            return false;
        }

        private string GetName(ReadOnlySpan<char> path)
        {
            var p = (ReadOnlySpan<char>)GetRelativePath(path);

            // remove file extension
            var ext = p.LastIndexOf('.');
            if (ext >= 0)
                p = p.Slice(0, ext);

            return p.ToString();
        }

        private unsafe string GetRelativePath(ReadOnlySpan<char> path)
        {
            Span<char> r = stackalloc char[AssetsPath.Length];
            Span<char> p = stackalloc char[path.Length];

            ((ReadOnlySpan<char>)AssetsPath).CopyTo(r);
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

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
